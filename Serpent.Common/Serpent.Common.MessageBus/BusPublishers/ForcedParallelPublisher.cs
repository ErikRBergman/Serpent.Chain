// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public class ForcedParallelPublisher<T> : BusPublisher<T>
    {
        public static BusPublisher<T> Default { get; } = new ForcedParallelPublisher<T>();

        public override Task PublishAsync(IEnumerable<IMessageHandler<T>> subscriptions, T message, CancellationToken token)
        {
            foreach (var subscription in subscriptions)
            {
                Task.Run(() => subscription.HandleMessageAsync(message, token), token);
            }

            return Task.CompletedTask;
        }
    }
}