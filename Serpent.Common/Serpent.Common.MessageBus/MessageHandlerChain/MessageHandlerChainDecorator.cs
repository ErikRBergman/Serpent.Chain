namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class MessageHandlerChainDecorator<TMessageType>
    {
        public abstract Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken);
    }
}