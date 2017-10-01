namespace Serpent.Common.MessageBus.Models
{
    using System.Threading;
    using System.Threading.Tasks;

    internal struct MessageAndCompletionContainer<TMessageType>
    {
        public MessageAndCompletionContainer(TMessageType message, TaskCompletionSource<TMessageType> taskCompletionSource, CancellationToken cancellationToken)
        {
            this.Message = message;
            this.TaskCompletionSource = taskCompletionSource;
            this.CancellationToken = cancellationToken;
        }

        public TMessageType Message { get; private set; }

        public TaskCompletionSource<TMessageType> TaskCompletionSource { get; private set; }

        public CancellationToken CancellationToken { get; private set; }
    }
}