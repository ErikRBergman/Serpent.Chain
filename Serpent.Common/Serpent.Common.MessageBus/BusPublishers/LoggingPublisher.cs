// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class LoggingPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        private readonly BusPublisher<TMessageType> busPublisher;

        private readonly Func<IEnumerable<ISubscription<TMessageType>>, TMessageType, BusPublisher<TMessageType>, Task> publishFunc;

        public LoggingPublisher(BusPublisher<TMessageType> busPublisher, Func<IEnumerable<ISubscription<TMessageType>>, TMessageType, BusPublisher<TMessageType>, Task> publishFunc)
        {
            this.busPublisher = busPublisher;
            this.publishFunc = publishFunc;
        }

        public LoggingPublisher(BusPublisher<TMessageType> busPublisher, Func<TMessageType, BusPublisher<TMessageType>, Task> publishFunc)
        {
            this.busPublisher = busPublisher;
            this.publishFunc = (subscriptions, message, publisher) => publishFunc(message, busPublisher);
        }

        public LoggingPublisher(BusPublisher<TMessageType> busPublisher, Action<TMessageType> publishFunc)
        {
            this.busPublisher = busPublisher;
            this.publishFunc = (subscriptions, message, publisher) =>
                {
                    publishFunc(message);
                    return this.busPublisher.PublishAsync(subscriptions, message);
                };
        }

        public override Task PublishAsync(IEnumerable<ISubscription<TMessageType>> subscriptions, TMessageType message)
        {
            return this.publishFunc(subscriptions, message, this.busPublisher);
        }
    }
}