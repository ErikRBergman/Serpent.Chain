namespace Serpent.Common.MessageBus.BusPublishers
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Helpers;

    internal class MessageHandlerPublishDispatch<TMessgeType> : IMessageBusSubscriptions<TMessgeType>
    {
        public Func<TMessgeType, Task> InvocationFunc { get; private set; }

        public IMessageBusSubscription Subscribe(Func<TMessgeType, Task> invocationFunc)
        {
            this.InvocationFunc = invocationFunc;
            return new NullMessageBusSubscription();
        }
    }
}