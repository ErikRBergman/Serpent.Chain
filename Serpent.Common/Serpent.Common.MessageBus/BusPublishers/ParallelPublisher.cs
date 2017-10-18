// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class ParallelPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        public static BusPublisher<TMessageType> Default { get; } = new ParallelPublisher<TMessageType>();

        public override Task PublishAsync(IEnumerable<Func<TMessageType, CancellationToken, Task>> handlers, TMessageType message, CancellationToken token)
        {
            return Task.WhenAll(
                handlers.Select(
                    handler => handler(message, token)));
        }
    }
}