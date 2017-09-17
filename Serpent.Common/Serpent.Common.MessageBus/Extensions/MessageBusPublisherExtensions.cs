namespace Serpent.Common.MessageBus
{
    using System.Threading.Tasks;

    public static class MessageBusPublisherExtensions
    {
        public static void PublishWithoutFeedback<TMessageType>(this IMessageBusPublisher<TMessageType> messageBus, TMessageType message)
        {
            Task.Run(() => messageBus.PublishAsync(message));
        }

        public static Task PublishAsync<TMessageType>(this IMessageBusPublisher<TMessageType> messageBus)
            where TMessageType : new()
        {
            return messageBus.PublishAsync(new TMessageType());
        }
    }
}