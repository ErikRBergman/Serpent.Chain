namespace Serpent.Common.MessageBus
{
    using System.Threading.Tasks;

    public static class MessageBusPublisherExtensions
    {
        public static void PublishWithoutFeedback<T>(this IMessageBusPublisher<T> messageBus, T message)
        {
            Task.Run(() => messageBus.PublishAsync(message));
        }
    }
}