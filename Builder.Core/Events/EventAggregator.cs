using Builder.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace Builder.Core.Events
{
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<WeakReference>> _subscribers;

        private readonly object _lock;

        public EventAggregator()
        {
            Logger.Initializing(this);
            _subscribers = new Dictionary<Type, List<WeakReference>>();
            _lock = new object();
        }

        public void Send<TArgs>(TArgs args) where TArgs : EventBase
        {
            Type subsriberType = typeof(ISubscriber<>).MakeGenericType(typeof(TArgs));
            List<WeakReference> subscriberList = GetSubscriberList(subsriberType);
            List<WeakReference> list = new List<WeakReference>();
            foreach (WeakReference item in subscriberList)
            {
                if (item.IsAlive)
                {
                    ISubscriber<TArgs> subscriber = (ISubscriber<TArgs>)item.Target;
                    InvokeSubscriberEvent(args, subscriber);
                }
                else
                {
                    list.Add(item);
                }
            }
            if (!list.Any())
            {
                return;
            }
            lock (_lock)
            {
                foreach (WeakReference item2 in list)
                {
                    subscriberList.Remove(item2);
                }
            }
        }

        public void Subscribe(object subscriber)
        {
            lock (_lock)
            {
                IEnumerable<Type> enumerable = from i in subscriber.GetType().GetInterfaces()
                                               where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscriber<>)
                                               select i;
                WeakReference item = new WeakReference(subscriber);
                foreach (Type item2 in enumerable)
                {
                    GetSubscriberList(item2).Add(item);
                }
            }
        }

        private void InvokeSubscriberEvent<TArgs>(TArgs args, ISubscriber<TArgs> subscriber) where TArgs : EventBase
        {
            (SynchronizationContext.Current ?? new SynchronizationContext()).Post(delegate
            {
                subscriber.OnHandleEvent(args);
            }, null);
        }

        private List<WeakReference> GetSubscriberList(Type subsriberType)
        {
            List<WeakReference> value = null;
            lock (_lock)
            {
                if (!_subscribers.TryGetValue(subsriberType, out value))
                {
                    value = new List<WeakReference>();
                    _subscribers.Add(subsriberType, value);
                }
            }
            return value;
        }
    }
}
