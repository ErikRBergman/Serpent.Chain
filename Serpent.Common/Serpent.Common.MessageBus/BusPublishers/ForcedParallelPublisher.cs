// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal class ForcedParallelPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        public static BusPublisher<TMessageType> Default { get; } = new ForcedParallelPublisher<TMessageType>();

        public override Task PublishAsync(IEnumerable<Func<TMessageType, CancellationToken, Task>> handlers, TMessageType message, CancellationToken token)
        {
            foreach (var subscription in handlers)
            {
                Task.Run(() => subscription(message, token), token);
            }

            return Task.CompletedTask;
        }
    }
}