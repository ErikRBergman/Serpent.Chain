namespace Serpent.Common.MessageBus.Extras
{
    using System.Threading;
    using System.Threading.Tasks;

    public struct PublisherBridge<T> : IMessageBusPublisher<T>
    {
        private readonly IMessageBus<T> bus;

        public PublisherBridge(IMessageBus<T> bus)
        {
            this.bus = bus;
        }

        public Task PublishAsync(T message, CancellationToken cancellationToken)
        {
            return this.bus.PublishAsync(message, cancellationToken);
        }
    }
}