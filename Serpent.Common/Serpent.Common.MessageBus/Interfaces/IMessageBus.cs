// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    public interface IMessageBus<TMessageType> : IMessageBusPublisher<TMessageType>, IMessageBusSubscriptions<TMessageType>
    {
    }
}