// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMessageSubscriptionBuilder<TBaseType>
    {
        IMessageSubscriptionBuilder<TBaseType> Map<T>(Func<T, CancellationToken, Task> invocationFunc)
            where T : TBaseType;
    }
}