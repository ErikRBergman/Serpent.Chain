namespace Serpent.Common.MessageBus
{
    using System.Threading.Tasks;

    public static class MessageBusPublisherExtensions
    {
        public static void PublishEventWithoutFeedback<T>(this IMessageBusPublisher<T> messageBus, T eventData)
        {
            Task.Run(() => messageBus.PublishEventAsync(eventData));
        }
    }
}