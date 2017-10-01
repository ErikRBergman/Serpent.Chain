// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal class MessageBusSubscriptionBuilder<TBaseType> : IMessageSubscriptionBuilder<TBaseType>
    {
        private readonly Dictionary<Type, Func<TBaseType, CancellationToken, Task>> invocationMap = new Dictionary<Type, Func<TBaseType, CancellationToken, Task>>();

        public int Count => this.invocationMap.Count;

        public IReadOnlyDictionary<Type, Func<TBaseType, CancellationToken, Task>> InvocationMap => this.invocationMap;

        public IMessageSubscriptionBuilder<TBaseType> Map<T>(Func<T, CancellationToken, Task> invocationFunc)
            where T : TBaseType
        {
            this.invocationMap[typeof(T)] = (message, token) => invocationFunc((T)message, token);
            return this;
        }
    }
}