// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Threading.Tasks;

    public interface IMessageBusPublisher<TMessage>
    {
        Task PublishAsync(TMessage message);
    }
}