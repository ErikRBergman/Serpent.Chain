namespace Serpent.Common.MessageBus
{
    public interface IMessageBus<TMessage> : IMessageBusPublisher<TMessage>, IMessageBusSubscriber<TMessage>
    {
    }
}