using Autofac;
using FWF.ComponentModel;
using FWF.Logging;
using FWF.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWF.CQRS
{
    #region Wrapper classes

    internal abstract class CommandHandlerItem
    {
        public abstract ICommandResponse Handle(ISecurityContext securityContext, ICommand command);
    }
    internal class CommandHandlerItem<T> : CommandHandlerItem where T : ICommand
    {
        private readonly ICommandHandler<T> _handler;

        public CommandHandlerItem(ICommandHandler<T> handler)
        {
            _handler = handler;
        }

        public override ICommandResponse Handle(ISecurityContext securityContext, ICommand command)
        {
            return _handler.Handle(securityContext, (T)command);
        }
    }

    internal abstract class QueryHandlerItem
    {
        public abstract IQueryResponse Handle(ISecurityContext securityContext, IQuery query);
    }
    internal class QueryHandlerItem<T> : QueryHandlerItem where T : IQuery
    {
        private readonly IQueryHandler<T> _handler;

        public QueryHandlerItem(IQueryHandler<T> handler)
        {
            _handler = handler;
        }

        public override IQueryResponse Handle(ISecurityContext securityContext, IQuery query)
        {
            return _handler.Handle(securityContext, (T)query);
        }
    }

    internal abstract class EventHandlerItem
    {
        public abstract void Handle(ISecurityContext securityContext, IEvent @event);
    }
    internal class EventHandlerItem<T> : EventHandlerItem where T : IEvent
    {
        private readonly IEventHandler<T> _handler;

        public EventHandlerItem(IEventHandler<T> handler)
        {
            _handler = handler;
        }

        public override void Handle(ISecurityContext securityContext, IEvent @event)
        {
            _handler.Handle(securityContext, (T)@event);
        }
    }

    #endregion

    internal class CqrsLogicHandler : Startable, ICqrsLogicHandler, IEventPublisherSubscription
        
    {
        // Stores the applicable handler for the incoming type
        private readonly IDictionary<int, HashSet<QueryHandlerItem>> _queryHandlers = new Dictionary<int, HashSet<QueryHandlerItem>>();
        private readonly IDictionary<int, HashSet<CommandHandlerItem>> _commandHandlers = new Dictionary<int, HashSet<CommandHandlerItem>>();
        private readonly IDictionary<int, HashSet<EventHandlerItem>> _eventHandlers = new Dictionary<int, HashSet<EventHandlerItem>>();

        private readonly Type _genericQueryHandlerType = typeof(IQueryHandler<>);
        private readonly Type _genericCommandHandlerType = typeof(ICommandHandler<>);
        private readonly Type _genericEventHandlerType = typeof(IEventHandler<>);

        private readonly IComponentContext _componentContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILog _log;
        
        public CqrsLogicHandler(
            IComponentContext componentContext,
            IEventPublisher eventPublisher,
            ILogFactory logFactory
            )
        {
            _componentContext = componentContext;
            _eventPublisher = eventPublisher;

            _log = logFactory.CreateForType(this);

            // Subscribe myself to events fired by the publisher
            _eventPublisher.Attach(this);
        }

        protected override void OnStart()
        {
            var allQueryHandlers = _componentContext.ResolveAll<IQueryHandler>();
            var allCommandHandlers = _componentContext.ResolveAll<ICommandHandler>();
            var allEventHandlers = _componentContext.ResolveAll<IEventHandler>();

            foreach (var handler in allQueryHandlers)
            {
                // NOTE: It is possible that the same component can handle multiple requests.  Get 
                // all interfaces for the type to identify each of these abilities.
                Type[] genericInterfaces = handler.GetType().GetInterfaces();

                foreach (var genericInterface in genericInterfaces)
                {
                    //
                    if (typeof(IQueryHandler) == genericInterface)
                    {
                        continue;
                    }

                    // If the interface is not derived from T, then ignore
                    if (!typeof(IQueryHandler).IsAssignableFrom(genericInterface))
                    {
                        continue;
                    }

                    var queryType = genericInterface.GetGenericArguments()[0];
                    var typeHashCode = genericInterface.GetHashCode();

                    if (!_queryHandlers.ContainsKey(typeHashCode))
                    {
                        _queryHandlers.Add(typeHashCode, new HashSet<QueryHandlerItem>());
                    }

                    // Create generic wrapper component
                    var item = (QueryHandlerItem)_componentContext.Resolve(
                        typeof(QueryHandlerItem<>).MakeGenericType(queryType),
                        new PositionalParameter(0, handler)
                        );

                    _queryHandlers[typeHashCode].Add(item);
                }
            }

            foreach (var handler in allCommandHandlers)
            {
                // NOTE: It is possible that the same component can handle multiple requests.  Get 
                // all interfaces for the type to identify each of these abilities.
                Type[] genericInterfaces = handler.GetType().GetInterfaces();

                foreach (var genericInterface in genericInterfaces)
                {
                    //
                    if (typeof(ICommandHandler) == genericInterface)
                    {
                        continue;
                    }

                    // If the interface is not derived from T, then ignore
                    if (!typeof(ICommandHandler).IsAssignableFrom(genericInterface))
                    {
                        continue;
                    }

                    var commandType = genericInterface.GetGenericArguments()[0];
                    var typeHashCode = genericInterface.GetHashCode();

                    if (!_commandHandlers.ContainsKey(typeHashCode))
                    {
                        _commandHandlers.Add(typeHashCode, new HashSet<CommandHandlerItem>());
                    }

                    // Create generic wrapper component
                    var item = (CommandHandlerItem)_componentContext.Resolve(
                        typeof(CommandHandlerItem<>).MakeGenericType(commandType), 
                        new PositionalParameter(0, handler)
                        );

                    _commandHandlers[typeHashCode].Add(item);
                }
            }

            foreach (var handler in allEventHandlers)
            {
                // NOTE: It is possible that the same component can handle multiple requests.  Get 
                // all interfaces for the type to identify each of these abilities.
                Type[] genericInterfaces = handler.GetType().GetInterfaces();

                foreach (var genericInterface in genericInterfaces)
                {
                    //
                    if (typeof(IEventHandler) == genericInterface)
                    {
                        continue;
                    }

                    // If the interface is not derived from T, then ignore
                    if (!typeof(IEventHandler).IsAssignableFrom(genericInterface))
                    {
                        continue;
                    }

                    var eventType = genericInterface.GetGenericArguments()[0];
                    var typeHashCode = genericInterface.GetHashCode();

                    if (!_eventHandlers.ContainsKey(typeHashCode))
                    {
                        _eventHandlers.Add(typeHashCode, new HashSet<EventHandlerItem>());
                    }

                    // Create generic wrapper component
                    var item = (EventHandlerItem)_componentContext.Resolve(
                        typeof(EventHandlerItem<>).MakeGenericType(eventType),
                        new PositionalParameter(0, handler)
                        );

                    _eventHandlers[typeHashCode].Add(item);
                }
            }
        }

        protected override void OnStop()
        {
            _queryHandlers.Clear();
            _commandHandlers.Clear();
            _eventHandlers.Clear();
        }

        public void Forward(IEvent @event)
        {
            Handle(new NoOpSecurityContext(), @event);
        }

        public IQueryResponse Handle(ISecurityContext securityContext, IQuery query)
        {
            if (!this.IsRunning)
            {
                throw new Exception("Must start component first - CqrsLogicExecutor");
            }

            if (ReferenceEquals(query, null))
            {
                throw new ArgumentNullException("query");
            }

            var validationResults = query.Validate();

            if (validationResults.Any())
            {
                if (_log.IsLevelEnabled(LogLevel.Verbose))
                {
                    _log.VerboseFormat(
                        "Event data did not pass validation: {0}\r\n{1}",
                        query.ToJsonString(),
                        validationResults.RenderValidationErrors()
                        );
                }

#if DEBUG
                throw new Exception(
                    string.Format(
                        "Event data did not pass validation: {0}\r\n{1}",
                        query.ToJsonString(),
                        validationResults.RenderValidationErrors()
                        )
                        );
#else
                return null;
#endif
            }

            // Get the specific type of the event
            var payloadType = query.GetType();

            // Get the specific interface to handle the event
            var requestHandlerType = _genericQueryHandlerType.MakeGenericType(payloadType);

            if (_log.IsLevelEnabled(LogLevel.Verbose))
            {
                _log.VerboseFormat("Publishing {0}", payloadType.Name);
            }

            var typeHashCode = requestHandlerType.GetHashCode();

            if (!_queryHandlers.ContainsKey(typeHashCode))
            {
                // TODO: Log
                return null;
            }

            var listApplicableHandlers = _queryHandlers[typeHashCode];

            if (!listApplicableHandlers.Any())
            {
                // TODO: Log
                return null;
            }

            var handler = listApplicableHandlers.First();

            if (_log.IsLevelEnabled(LogLevel.Verbose))
            {
                _log.VerboseFormat(
                    "Handling {0} with {1}",
                    payloadType.Name,
                    handler.GetType().Name
                    );
            }

            IQueryResponse response = null;

            try
            {
                response = handler.Handle(securityContext, query);
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }

            return response;
        }

        public ICommandResponse Handle(ISecurityContext securityContext, ICommand command)
        {
            if (!this.IsRunning)
            {
                throw new Exception("Must start component first - CqrsLogicExecutor");
            }

            if (ReferenceEquals(command, null))
            {
                throw new ArgumentNullException("command");
            }

            var validationResults = command.Validate();

            if (validationResults.Any())
            {
                if (_log.IsLevelEnabled(LogLevel.Verbose))
                {
                    _log.VerboseFormat(
                        "Data did not pass validation: {0}\r\n{1}",
                        command.ToJsonString(),
                        validationResults.RenderValidationErrors()
                        );
                }

                var msg = string.Format(
                        "Data did not pass validation: {0}",
                        validationResults.RenderValidationErrors()
                        );

                throw new ValidationException(validationResults, msg);
            }

            // Get the specific type of the event
            var payloadType = command.GetType();

            // Get the specific interface to handle the event
            var requestHandlerType = _genericCommandHandlerType.MakeGenericType(payloadType);

            if (_log.IsLevelEnabled(LogLevel.Verbose))
            {
                _log.VerboseFormat("Publishing {0}", payloadType.Name);
            }

            var typeHashCode = requestHandlerType.GetHashCode();

            if (!_commandHandlers.ContainsKey(typeHashCode))
            {
                // TODO: Log
                return null;
            }

            var listApplicableHandlers = _commandHandlers[typeHashCode];

            if (!listApplicableHandlers.Any())
            {
                // TODO: Log
                return null;
            }

            var handler = listApplicableHandlers.First();

            if (_log.IsLevelEnabled(LogLevel.Verbose))
            {
                _log.VerboseFormat(
                    "Handling {0} with {1}",
                    payloadType.Name,
                    handler.GetType().Name
                    );
            }

            ICommandResponse response = null;

            try
            {
                response = handler.Handle(securityContext, command);
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);

                throw;
            }

            return response;
        }

        public void Handle(ISecurityContext securityContext, IEvent @event)
        {
            if (!this.IsRunning)
            {
                throw new Exception("Must start component first - CqrsLogicExecutor");
            }

            if (ReferenceEquals(@event, null))
            {
                throw new ArgumentNullException("event");
            }

            var validationResults = @event.Validate();

            if (validationResults.Any())
            {
                if (_log.IsLevelEnabled(LogLevel.Verbose))
                {
                    _log.VerboseFormat(
                        "Data did not pass validation: {0}\r\n{1}",
                        @event.ToJsonString(),
                        validationResults.RenderValidationErrors()
                        );
                }

                var msg = string.Format(
                        "Data did not pass validation: {0}",
                        validationResults.RenderValidationErrors()
                        );

                throw new ValidationException(validationResults, msg);
            }

            // Get the specific type of the event
            var payloadType = @event.GetType();

            // Get the specific interface to handle the event
            var requestHandlerType = _genericEventHandlerType.MakeGenericType(payloadType);

            if (_log.IsLevelEnabled(LogLevel.Verbose))
            {
                _log.VerboseFormat("Publishing {0}", payloadType.Name);
            }

            var typeHashCode = requestHandlerType.GetHashCode();

            if (!_eventHandlers.ContainsKey(typeHashCode))
            {
                // TODO: Log
                return;
            }

            var listApplicableHandlers = _eventHandlers[typeHashCode];

            if (!listApplicableHandlers.Any())
            {
                // TODO: Log
                return;
            }

            try
            {
                foreach (var handler in listApplicableHandlers)
                {
                    if (_log.IsLevelEnabled(LogLevel.Verbose))
                    {
                        _log.VerboseFormat(
                            "Handling {0} with {1}",
                            payloadType.Name,
                            handler.GetType().Name
                            );
                    }

                    handler.Handle(securityContext, @event);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);

                throw ex;
            }
        }

    }
}
