// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Threading.Tasks;

    public interface IMessageBusPublisher<in TMessageType>
    {
        Task PublishAsync(TMessageType message);
    }
}