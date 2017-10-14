namespace Serpent.Common.MessageBus.Extras
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A bridge to allow registering IMessageBusPublisher to the same IMessageBus with simpler IOC containers, like the one in ASP NET Core
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public struct PublisherBridge<TMessageType> : IMessageBusPublisher<TMessageType>
    {
        private readonly IMessageBus<TMessageType> bus;

        public PublisherBridge(IMessageBus<TMessageType> bus)
        {
            this.bus = bus;
        }

        public Task PublishAsync(TMessageType message, CancellationToken cancellationToken)
        {
            return this.bus.PublishAsync(message, cancellationToken);
        }
    }
}