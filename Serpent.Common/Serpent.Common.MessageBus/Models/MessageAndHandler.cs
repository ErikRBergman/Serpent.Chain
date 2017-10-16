namespace Serpent.Common.MessageBus.Models
{
    using Serpent.Common.MessageBus.Interfaces;

    public struct MessageAndHandler<TMessageType>
    {
        public MessageAndHandler(TMessageType message, IMessageHandler<TMessageType> subscription)
        {
            this.Message = message;
            this.Subscription = subscription;
        }

        public TMessageType Message { get; }

        public IMessageHandler<TMessageType> Subscription { get; }
    }
}