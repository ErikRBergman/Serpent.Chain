// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Threading.Tasks;

    public static class MessageBusPublisherExtensions
    {
        public static void PublishWithoutFeedback<TMessageType>(this IMessageBusPublisher<TMessageType> messageBus, TMessageType message)
        {
            Task.Run(() => messageBus.PublishAsync(message));
        }

        public static void Publish<TMessageType>(this IMessageBusPublisher<TMessageType> messageBus, TMessageType message)
        {
            // This call is not awaited, and that's the purpose. If it can finish synchronously, let it, otherwise, return control to the caller.
            messageBus.PublishAsync(message);
        }

        public static void Publish<TMessageType>(this IMessageBusPublisher<TMessageType> messageBus)
            where TMessageType : new()
        {
            // This call is not awaited, and that's the purpose. If it can finish synchronously, let it, otherwise, return control to the caller.
            messageBus.PublishAsync(new TMessageType());
        }

        public static Task PublishAsync<TMessageType>(this IMessageBusPublisher<TMessageType> messageBus)
            where TMessageType : new()
        {
            return messageBus.PublishAsync(new TMessageType());
        }
    }
}