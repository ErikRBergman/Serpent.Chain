// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class FuncPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        private readonly Func<IEnumerable<ISubscription<TMessageType>>, TMessageType, Task> publishFunc;

        public FuncPublisher(Func<IEnumerable<ISubscription<TMessageType>>, TMessageType, Task> publishFunc)
        {
            this.publishFunc = publishFunc;
        }

        public override Task PublishAsync(IEnumerable<ISubscription<TMessageType>> subscriptions, TMessageType message, CancellationToken token)
        {
            return this.publishFunc(subscriptions, message);
        }
    }
}