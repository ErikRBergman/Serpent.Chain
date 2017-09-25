// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    public interface IMessageBusSubscriber<out TMessageType>
    {
        IMessageBusSubscription Subscribe(Func<TMessageType, Task> invocationFunc);
    }
}