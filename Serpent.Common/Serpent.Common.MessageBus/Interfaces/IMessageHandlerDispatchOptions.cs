namespace Serpent.Common.MessageBus.Interfaces
{
    public interface IMessageHandlerDispatchOptions<TMessageType>
    {
        ConcurrentMessageBusOptions<TMessageType> Options { get; }
    }
}