// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal class FuncPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        private readonly Func<IEnumerable<Func<TMessageType, CancellationToken, Task>>, TMessageType, Task> publishFunc;

        public FuncPublisher(Func<IEnumerable<Func<TMessageType, CancellationToken, Task>>, TMessageType, Task> publishFunc)
        {
            this.publishFunc = publishFunc;
        }

        public override Task PublishAsync(IEnumerable<Func<TMessageType, CancellationToken, Task>> handlers, TMessageType message, CancellationToken token)
        {
            return this.publishFunc(handlers, message);
        }
    }
}