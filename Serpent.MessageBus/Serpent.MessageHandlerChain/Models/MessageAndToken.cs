namespace Serpent.MessageHandlerChain.Models
{
    using System.Threading;

    internal struct MessageAndToken<TMessageType>
    {
        public MessageAndToken(TMessageType message, CancellationToken token)
        {
            this.Message = message;
            this.Token = token;
        }

        public TMessageType Message { get; }

        public CancellationToken Token { get; }
    }
}