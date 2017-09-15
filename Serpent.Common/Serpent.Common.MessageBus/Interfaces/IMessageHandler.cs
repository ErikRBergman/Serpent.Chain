namespace Serpent.Common.MessageBus.Interfaces
{
    using System.Threading.Tasks;

    public interface IMessageHandler<TMessage>
    {
        Task HandleMessageAsync(TMessage message);
    }
}