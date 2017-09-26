namespace Serpent.Common.MessageBus.Models
{
    using System.Threading.Tasks;

    internal struct MessageAndCompletionContainer<TMessageType>
    {
        public MessageAndCompletionContainer(TMessageType message, TaskCompletionSource<TMessageType> taskCompletionSource)
        {
            this.Message = message;
            this.TaskCompletionSource = taskCompletionSource;
        }

        public TMessageType Message { get; private set; }

        public TaskCompletionSource<TMessageType> TaskCompletionSource { get; private set; }
    }
}