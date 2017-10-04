namespace Serpent.Common.MessageBus.Interfaces
{
    using System.Threading.Tasks;

    public interface ISimpleMessageHandler<in TMessageType>
    {
        Task HandleMessageAsync(TMessageType message);
    }
}