namespace Serpent.Chain.Models
{
    using System.Threading.Tasks;

    public struct MessageWithResult<TMessageType, TResultType>
    {
        public TMessageType Message { get; set; }

        public TaskCompletionSource<TResultType> Result { get; set; }
    }
}
