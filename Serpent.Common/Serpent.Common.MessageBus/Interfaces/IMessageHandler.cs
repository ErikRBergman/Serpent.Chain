namespace Serpent.Common.MessageBus.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMessageHandler<in TMessageType>
    {
        Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken);
    }
}