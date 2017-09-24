// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    public interface IMessageSubscriptionBuilder<TBaseType>
    {
        IMessageSubscriptionBuilder<TBaseType> Map<T>(Func<T, Task> invocationFunc)
            where T : TBaseType;
    }
}