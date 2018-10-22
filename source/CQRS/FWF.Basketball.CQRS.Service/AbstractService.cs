using Autofac;
using FWF.Configuration;
using FWF.CQRS;
using FWF.Json;
using FWF.Logging;
using FWF.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using FWF.Basketball.CQRS.Service.Web;

namespace FWF.Basketball.CQRS.Service
{
    public abstract class AbstractService : Startable
    {
        private IWebHost _webHost;
        private Uri _hostUri;

        private readonly ICqrsLogicHandler _logicHandler;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalTcpPortManager _localTcpPortManager;
        private readonly IAppSettings _appSettings;
        private readonly ILog _log;

        public AbstractService(
            IComponentContext componentContext,
            ILogFactory logFactory
            )
        {
            _logicHandler = componentContext.Resolve<ICqrsLogicHandler>();
            _eventPublisher = componentContext.Resolve<IEventPublisher>();
            _localTcpPortManager = componentContext.Resolve<ILocalTcpPortManager>();
            _appSettings = componentContext.Resolve<IAppSettings>();

            _log = logFactory.CreateForType(this);
        }

        protected override void OnStart()
        {
            // Update some basic settings
            _appSettings.AddOrUpdate(CommonAppSettingsKeys.ClientIdSetting.Name, Guid.NewGuid().ToString());
            _appSettings.AddOrUpdate(CommonAppSettingsKeys.ClientNameSetting.Name, "CQRS Service");

            // Start dependent components
            _eventPublisher.Start();
            _logicHandler.Start();

            // Get a TCP port that is available
            var portReservation = _localTcpPortManager.GetNextAvailablePort();

            var hostPort = portReservation.Port;
            var sslPort = portReservation.Port + 1;

            // Update the local setting to begin hosting with the port
            _appSettings.AddOrUpdate(CommonAppSettingsKeys.HttpHostPortSetting.Name, portReservation.Port.ToString());


            try
            {
                // Expose API endpoint
                if (_webHost.IsNotNull())
                {
                    _webHost.Dispose();
                }

                var builder = WebHost.CreateDefaultBuilder();

                //
                _hostUri = new UriBuilder(
                    "https",
                    "localhost",
                    sslPort
                    ).Uri;

                builder.UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, hostPort);
                    options.Listen(IPAddress.Loopback, sslPort, listenOptions =>
                    {
                        listenOptions.UseHttps(StoreName.My, "localhost");
                    });
                }
                );

                builder.ConfigureServices(ConfigureServices);
                builder.Configure(Configure);

                _webHost = builder.Build();

                try
                {
                    _webHost.Start();
                }
                catch (Exception ex)
                {
                    _log.Error(ex, ex.Message);

                    throw;
                }
            }
            finally
            {
                portReservation.Dispose();
            }

            _log.InfoFormat(
                "Listening on: http://localhost:{0}/ and https://localhost:{1}/",
                hostPort,
                sslPort
                );
        }

        protected override void OnStop()
        {
            _webHost.Dispose();

            _logicHandler.Stop();
            _eventPublisher.Stop();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
        }

        protected abstract void Configure(IApplicationBuilder app);


    }
}

