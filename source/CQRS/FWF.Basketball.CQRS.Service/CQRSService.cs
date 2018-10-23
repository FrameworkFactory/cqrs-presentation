using Autofac;
using FWF.Basketball.CQRS.Data;
using FWF.Basketball.CQRS.Service.Web;
using FWF.Basketball.Logic;
using FWF.Basketball.Logic.Data;
using FWF.CQRS;
using FWF.Json;
using FWF.Logging;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FWF.Basketball.CQRS.Service
{
    internal class CQRSService : AbstractService, IEventPublisherSubscription
    {

        private List<IHttpRequestHandler> _handlers = new List<IHttpRequestHandler>();

        private List<WebSocket> _sockets = new List<WebSocket>();
        private volatile object _lockObject = new object();

        private readonly IGamePlayEngine _gamePlayEngine;
        private readonly IGamePlayListener _gamePlayListener;

        private readonly IReadCacheDataRepository _readCacheDataRepository;
        private readonly IGameDataRepository _gameDataRepository;

        private readonly ICqrsLogicHandler _logicHandler;
        private readonly ILog _log;

        public CQRSService(
            IGamePlayEngine gamePlayEngine,
            IGamePlayListener gamePlayListener,
            IReadCacheDataRepository readCacheDataRepository,
            IGameDataRepository gameDataRepository,
            ICqrsLogicHandler logicHandler,
            IEventPublisher eventPublisher,
            IComponentContext componentContext,
            ILogFactory logFactory
            ) : base(componentContext, logFactory)
        {
            _gamePlayEngine = gamePlayEngine;
            _gamePlayListener = gamePlayListener;
            _readCacheDataRepository = readCacheDataRepository;
            _gameDataRepository = gameDataRepository;

            _logicHandler = logicHandler;

            _handlers.AddRange(
                componentContext.ResolveAll<IHttpRequestHandler>()
                );

            _log = logFactory.CreateForType(this);

            // Subscribe myself to any event
            eventPublisher.Attach(this);
        }

        protected override void OnStart()
        {
            base.OnStart();

            _readCacheDataRepository.Start();
            _gameDataRepository.Start();

            _logicHandler.Start();

            _gamePlayEngine.Subscribe(_gamePlayListener);

            _gamePlayEngine.Start();

        }

        protected override void OnStop()
        {
            _gamePlayEngine.Stop();

            _logicHandler.Stop();

            _readCacheDataRepository.Stop();
            _gameDataRepository.Stop();

            base.OnStop();
        }

        protected override void Configure(IApplicationBuilder app)
        {
            app.UseResponseBuffering();
            //app.UseHsts();
            app.UseStaticFiles();
            //app.UseResponseCompression();

            // 
            //app.UseAuthentication();

            // 
            foreach (var handler in _handlers)
            {
                try
                {
                    app.Use(async (context, next) => await handler.Handle(context, next));
                }
                catch (Exception ex)
                {
                    _log.Error(ex, ex.Message);

                    break;
                }
            }

            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024,
            };
            app.UseWebSockets(webSocketOptions);


            app.Use(async (context, next) =>
            {
                var path = context.Request.Path;

                if ("/ws".EqualsIgnoreCase(path))
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                        // Add to local connection
                        lock (_lockObject)
                        {
                            _sockets.Add(webSocket);
                        }

                        // Leave
                        var buffer = new byte[1024 * 4];
                        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                        while (!result.CloseStatus.HasValue)
                        {
                            //await webSocket.SendAsync(
                            //    new ArraySegment<byte>(buffer, 0, result.Count),
                            //    result.MessageType,
                            //    result.EndOfMessage,
                            //    CancellationToken.None
                            //    );

                            result = await webSocket.ReceiveAsync(
                                new ArraySegment<byte>(buffer),
                                CancellationToken.None
                                );
                        }

                        // Add to local connection
                        lock (_lockObject)
                        {
                            _sockets.Remove(webSocket);
                        }

                        await webSocket.CloseAsync(
                            result.CloseStatus.Value,
                            result.CloseStatusDescription,
                            CancellationToken.None
                            );

                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });



            // Terminate the request
            app.Run(async (x) => await Task.CompletedTask);
        }

        public void Forward(IEvent entityEvent)
        {
            // We have received an event that we want to forward to all clients

            if (_sockets.Count == 0)
            {
                return;
            }

            byte[] jsonBits = null;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.ASCII, 8192, true))
            using (var jsonWriter = JSON.GetWriter(streamWriter))
            {
                jsonWriter.WriteStartObject();

                // In order to work better with Vuex, add some properties that route the data
                // to the correct action/namespace/store, etc.

                var action = entityEvent.GetType().Name;
                var camelCaseAction = Char.ToLowerInvariant(action[0]) + action.Substring(1);

                jsonWriter.WritePropertyName("action");
                jsonWriter.Write(camelCaseAction);

                jsonWriter.WritePropertyName("namespace");
                jsonWriter.Write("game");

                // Each event can have many properties - use Reflection to find each property and render to JSON
                var props = entityEvent.ParseProperties();

                foreach (var key in props.Keys)
                {
                    var propertyValue = props[key];

                    // No need to write the property if it is null
                    if (ReferenceEquals(propertyValue, null))
                    {
                        continue;
                    }

                    // Properties should be camel case
                    var camelCaseKey = Char.ToLowerInvariant(key[0]) + key.Substring(1);

                    jsonWriter.WritePropertyName(camelCaseKey);
                    jsonWriter.Write(props[key]);
                }

                jsonWriter.WriteEndObject();

                // Ensure all writes are committed
                jsonWriter.Flush();

                // Reset to the beginning
                memoryStream.Position = 0;

                jsonBits = memoryStream.ToArray();
            }

            lock (_lockObject)
            {
                var tasks = new List<Task>();

                foreach (var socket in _sockets)
                {
                    var task = socket.SendAsync(
                            new ArraySegment<byte>(jsonBits),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                            );

                    tasks.Add(task);
                }

                Task.WaitAll(tasks.ToArray());
            }

        }


    }
}
