namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System.Threading.Tasks;

    public abstract class MessageHandlerChainDecorator<T>
    {
        public abstract Task HandleMessageAsync(T message);
    }
}