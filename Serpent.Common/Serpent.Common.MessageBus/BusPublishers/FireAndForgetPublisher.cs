// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class FireAndForgetPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        private readonly BusPublisher<TMessageType> innerPublisher;

        public FireAndForgetPublisher(BusPublisher<TMessageType> innerPublisher = null)
        {
            this.innerPublisher = innerPublisher ?? ParallelPublisher<TMessageType>.Default;
        }

        public static BusPublisher<TMessageType> Default { get; } = new FireAndForgetPublisher<TMessageType>();

        public override Task PublishAsync(IEnumerable<ISubscription<TMessageType>> subscriptions, TMessageType message)
        {
            Task.Run(() => this.innerPublisher.PublishAsync(subscriptions, message));
            return Task.CompletedTask;
        }
    }
}