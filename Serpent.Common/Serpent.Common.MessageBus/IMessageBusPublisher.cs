namespace Serpent.Common.MessageBus
{
    using System.Threading.Tasks;

    public interface IMessageBusPublisher<TMessage>
    {
        Task PublishEventAsync(TMessage eventData);
    }
}
