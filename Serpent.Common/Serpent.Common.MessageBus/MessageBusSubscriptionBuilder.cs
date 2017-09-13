namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class MessageBusSubscriptionBuilder<TBaseType> : IMessageSubscriptionBuilder<TBaseType>
    {
        private readonly Dictionary<Type, Func<TBaseType, Task>> invocationMap = new Dictionary<Type, Func<TBaseType, Task>>();

        public int Count => this.invocationMap.Count;

        public IReadOnlyDictionary<Type, Func<TBaseType, Task>> InvocationMap => this.invocationMap;

        public IMessageSubscriptionBuilder<TBaseType> Map<T>(Func<T, Task> invocationFunc)
            where T : TBaseType
        {
            this.invocationMap[typeof(T)] = type => invocationFunc((T)type);
            return this;
        }
    }
}