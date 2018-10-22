using Autofac;
using FWF.Logging;
using FWF.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FWF.CQRS
{
    internal class EventPublisher : Startable, IEventPublisher
    {

        private HashSet<IEventPublisherSubscription> _listeners = new HashSet<IEventPublisherSubscription>();
        private volatile object _lockObject = new object();

        private readonly ConcurrentQueue<EventQueueItem> _queue = new ConcurrentQueue<EventQueueItem>();

        private readonly IComponentContext _componentContext;
        private readonly StartableThread _thread;
        private readonly ILog _log;

        private class EventQueueItem
        {
            public IEvent Event
            {
                get; set;
            }
        }

        public EventPublisher(
            IComponentContext componentContext,
            StartableThread startableThread,
            ILogFactory logFactory
            )
        {
            _componentContext = componentContext;
            _thread = startableThread;

            _log = logFactory.CreateForType(this);
            
            _thread.Name = "EventPublisher";
            _thread.ThreadLatency = TimeSpan.FromMilliseconds(150);
            _thread.ThreadLoop = ProcessEventQueue;
        }

        protected override void OnStart()
        {
            _thread.Start();
        }

        protected override void OnStop()
        {
            _thread.Stop();
            _listeners.Clear();
        }

        public void Attach(IEventPublisherSubscription eventSubscription)
        {
            lock (_lockObject)
            {
                if (!_listeners.Contains(eventSubscription))
                {
                    _listeners.Add(eventSubscription);
                }
            }
        }

        public void Detach(IEventPublisherSubscription eventSubscription)
        {
            lock (_lockObject)
            {
                if (_listeners.Contains(eventSubscription))
                {
                    _listeners.Remove(eventSubscription);
                }
            }
        }

        public void Publish(IEvent @event)
        {
            if (!this.IsRunning)
            {
                throw new Exception("Must start component first - EventPublisher");
            }

            if (ReferenceEquals(@event, null))
            {
                throw new ArgumentNullException("event");
            }

            if (!@event.EventTimestamp.HasValue)
            {
                @event.EventTimestamp = DateTime.UtcNow;
            }
            if (!@event.EventId.HasValue)
            {
                @event.EventId = Guid.NewGuid();
            }

            //
            var validationResults = @event.Validate();

            if (validationResults.Any())
            {
                if (_log.IsLevelEnabled(LogLevel.Verbose))
                {
                    _log.VerboseFormat(
                        "Event data did not pass validation: {0}\r\n{1}",
                        @event.ToJsonString(),
                        validationResults.RenderValidationErrors()
                        );
                }

#if DEBUG
                throw new Exception(
                    string.Format(
                        "Event data did not pass validation: {0}\r\n{1}",
                        @event.ToJsonString(),
                        validationResults.RenderValidationErrors()
                        )
                        );
#else
                return;
#endif
            }

            _queue.Enqueue(
                new EventQueueItem
                {
                    Event = @event
                }
                );
        }

        public void Publish(IEnumerable<IEvent> events)
        {
            // FIX: We will need to provide more bulk operations for events
            // when we have the kind of possible volume I thinking we will have...

            foreach (var @event in events)
            {
                Publish(@event);
            }
        }

        private async Task ProcessEventQueue(IThreadLoopEvent loopEvent)
        {
            while (true)
            {
                EventQueueItem item = null;

                var didDequeue = _queue.TryDequeue(out item);

                if (!didDequeue)
                {
                    return;
                }

                FireEvent(item.Event);

                if (loopEvent.IsCancelled)
                {
                    break;
                }
            }

            await Task.CompletedTask;
        }

        private void FireEvent(IEvent entityEvent)
        {
            // Process any viewers attached to this component

            lock (_lockObject)
            {
                foreach (var item in _listeners)
                {
                    try
                    {
                        item.Forward(entityEvent);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, ex.Message);
                    }
                }
            }
        }

    }

}


