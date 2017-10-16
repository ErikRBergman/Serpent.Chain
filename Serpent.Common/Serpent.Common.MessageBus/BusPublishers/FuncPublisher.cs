// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public class FuncPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        private readonly Func<IEnumerable<IMessageHandler<TMessageType>>, TMessageType, Task> publishFunc;

        public FuncPublisher(Func<IEnumerable<IMessageHandler<TMessageType>>, TMessageType, Task> publishFunc)
        {
            this.publishFunc = publishFunc;
        }

        public override Task PublishAsync(IEnumerable<IMessageHandler<TMessageType>> subscriptions, TMessageType message, CancellationToken token)
        {
            return this.publishFunc(subscriptions, message);
        }
    }
}