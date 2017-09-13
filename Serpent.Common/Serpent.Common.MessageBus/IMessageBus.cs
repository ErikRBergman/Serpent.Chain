namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    public interface IMessageBus<TMessage> : IMessageBusPublisher<TMessage>, IMessageBusSubscriber<TMessage>
    {
    }

    public interface IMessageBusSubscriber<TMessage>
    {
        IMessageBusSubscription Subscribe(Func<TMessage, Task> invocationFunc);
    }
}