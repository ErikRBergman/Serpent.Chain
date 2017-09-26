namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System.Threading.Tasks;

    public abstract class MessageHandlerChainDecorator<T>
    {
        public abstract Task HandleMessageAsync(T message);
    }
}