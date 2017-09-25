namespace Serpent.Common.MessageBus.Models
{
    using System.Collections.Generic;

    public struct MessageAndSubscriptions<TMessageType>
    {
        public MessageAndSubscriptions(TMessageType message, IEnumerable<ISubscription<TMessageType>> subscriptions)
        {
            this.Message = message;
            this.Subscriptions = subscriptions;
        }

        public TMessageType Message { get; }

        public IEnumerable<ISubscription<TMessageType>> Subscriptions { get; }
    }
}