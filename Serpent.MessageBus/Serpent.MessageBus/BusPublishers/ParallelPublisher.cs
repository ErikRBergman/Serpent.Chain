// ReSharper disable once CheckNamespace
namespace Serpent.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The parallel bus publisher type
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public class ParallelPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        /// <summary>
        /// A default bus publisher instance
        /// </summary>
        public static BusPublisher<TMessageType> Default { get; } = new ParallelPublisher<TMessageType>();

        /// <inheritdoc/>
        public override Task PublishAsync(IEnumerable<Func<TMessageType, CancellationToken, Task>> handlers, TMessageType message, CancellationToken token)
        {
            return Task.WhenAll(
                handlers.Select(
                    handler => handler(message, token)));
        }
    }
}