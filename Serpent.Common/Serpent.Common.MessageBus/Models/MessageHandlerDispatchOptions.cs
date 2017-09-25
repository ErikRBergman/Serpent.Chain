namespace Serpent.Common.MessageBus.Models
{
    using Serpent.Common.MessageBus.Interfaces;

    public struct MessageHandlerDispatchOptions<TMessageType> : IMessageHandlerDispatchOptions<TMessageType>
    {
        public MessageHandlerDispatchOptions(ConcurrentMessageBusOptions<TMessageType> options)
        {
            this.Options = options;
        }

        public ConcurrentMessageBusOptions<TMessageType> Options { get; }
    }
}