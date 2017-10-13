// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    /// <summary>
    /// The message bus interface. Used to both publish messages to a message bus and set up subscriptions.
    /// If you want to only publish messages, use <see cref="Serpent.Common.MessageBus.IMessageBusPublisher&lt;TMessageType&gt;">IMessageBusPublisher&lt;TMessageType&gt;</see> 
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IMessageBus<TMessageType> : IMessageBusPublisher<TMessageType>, IMessageBusSubscriptions<TMessageType>
    {
    }
}