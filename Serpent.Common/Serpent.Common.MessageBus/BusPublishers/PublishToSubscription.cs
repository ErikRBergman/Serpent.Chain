namespace Serpent.Common.MessageBus.BusPublishers
{
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Models;

    public class PublishToSubscription
    {
        public static Task PublishAsync<TMessageType>(MessageAndSubscription<TMessageType> messageAndSubscription)
        {
            var subscriptionHandlerFunc = messageAndSubscription.Subscription.SubscriptionHandlerFunc;

            // subscriptionHandlerFunc is null when refering to a weak reference that has been garbage collected
            return subscriptionHandlerFunc == null ? Task.CompletedTask : subscriptionHandlerFunc(messageAndSubscription.Message);
        }
    }
}