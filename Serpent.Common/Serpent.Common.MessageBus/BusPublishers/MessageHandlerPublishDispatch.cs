namespace Serpent.Common.MessageBus.BusPublishers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Helpers;

    internal class MessageHandlerPublishDispatch<TMessgeType> : IMessageBusSubscriptions<TMessgeType>
    {
        public Func<TMessgeType, CancellationToken, Task> InvocationFunc { get; private set; }

        public IMessageBusSubscription Subscribe(Func<TMessgeType, CancellationToken, Task> invocationFunc)
        {
            this.InvocationFunc = invocationFunc;
            return new NullMessageBusSubscription();
        }
    }
}