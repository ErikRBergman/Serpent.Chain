namespace Serpent.Common.MessageBus.BusPublishers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Models;

    public class PublishToSubscription
    {
        public static Task PublishAsync<TMessageType>(MessageAndHandler<TMessageType> messageAndHandler, CancellationToken cancellationToken)
        {
            return messageAndHandler.Handler(messageAndHandler.Message, cancellationToken);
        }
    }
}