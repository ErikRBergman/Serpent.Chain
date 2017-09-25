namespace Serpent.Common.MessageBus.Interfaces
{
    using System.Threading.Tasks;

    public interface IMessageHandler<TMessageType>
    {
        Task HandleMessageAsync(TMessageType message);
    }
}