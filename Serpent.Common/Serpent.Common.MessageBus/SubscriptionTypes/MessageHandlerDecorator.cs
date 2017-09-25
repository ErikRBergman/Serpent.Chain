namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System.Threading.Tasks;

    public abstract class MessageHandlerDecorator<T>
    {
        public abstract Task HandleMessageAsync(T message);
    }
}