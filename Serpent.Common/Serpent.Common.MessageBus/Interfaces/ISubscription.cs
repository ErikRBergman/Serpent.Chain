// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISubscription<T>
    {
        Func<T, CancellationToken, Task> SubscriptionHandlerFunc { get; }
    }
}