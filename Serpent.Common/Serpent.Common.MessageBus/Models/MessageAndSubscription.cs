namespace Serpent.Common.MessageBus.Models
{
    public struct MessageAndSubscription<TMessageType>
    {
        public MessageAndSubscription(TMessageType message, ISubscription<TMessageType> subscription)
        {
            this.Message = message;
            this.Subscription = subscription;
        }

        public TMessageType Message { get; }

        public ISubscription<TMessageType> Subscription { get; }
    }
}