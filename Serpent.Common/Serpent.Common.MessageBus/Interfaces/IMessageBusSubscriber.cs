// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    public interface IMessageBusSubscriber<TMessage>
    {
        IMessageBusSubscription Subscribe(Func<TMessage, Task> invocationFunc);
    }
}