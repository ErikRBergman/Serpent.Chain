// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public class ParallelPublisher<T> : BusPublisher<T>
    {
        public static BusPublisher<T> Default { get; } = new ParallelPublisher<T>();

        public override Task PublishAsync(IEnumerable<IMessageHandler<T>> subscriptions, T message, CancellationToken token)
        {
            return Task.WhenAll(
                subscriptions.Select(
                    s => s.HandleMessageAsync(message, token)));
        }
    }
}