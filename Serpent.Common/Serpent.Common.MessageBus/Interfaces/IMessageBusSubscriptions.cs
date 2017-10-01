// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMessageBusSubscriptions<out TMessageType>
    {
        IMessageBusSubscription Subscribe(Func<TMessageType, CancellationToken, Task> invocationFunc);
    }
}