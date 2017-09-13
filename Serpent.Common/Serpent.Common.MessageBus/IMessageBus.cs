namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    public interface IMessageBus<TMessage>
    {
        Task PublishEventAsync(TMessage eventData);

        IMessageBusSubscription Subscribe(Func<TMessage, Task> invocationFunc);
    }
}