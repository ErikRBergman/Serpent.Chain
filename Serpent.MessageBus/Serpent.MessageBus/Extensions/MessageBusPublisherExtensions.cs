// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extensions to make it easier to publish messages
    /// </summary>
    public static class MessageBusPublisherExtensions
    {
        /// <summary>
        /// Publish a message without attempting to wait for it's delivery
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="publisher">The bus publisher</param>
        /// <param name="message">The message</param>
        public static void Publish<TMessageType>(this IMessageBusPublisher<TMessageType> publisher, TMessageType message)
        {
            // This call is not awaited, and that's the purpose. If it can finish synchronously, let it, otherwise, return control to the caller.
            publisher.PublishAsync(message);
        }

        /// <summary>
        /// Publish a new empty message without attempting to wait for it's delivery
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="publisher">The bus publisher</param>
        public static void Publish<TMessageType>(this IMessageBusPublisher<TMessageType> publisher)
            where TMessageType : new()
        {
            // This call is not awaited, and that's the purpose. If it can finish synchronously, let it, otherwise, return control to the caller.
            publisher.PublishAsync(new TMessageType());
        }

        /// <summary>
        /// Publish a new empty message, returning a Task that that succeeds when the message is handled
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="publisher">
        /// The bus publisher
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task PublishAsync<TMessageType>(this IMessageBusPublisher<TMessageType> publisher)
            where TMessageType : new()
        {
            return publisher.PublishAsync(new TMessageType());
        }

        /// <summary>
        /// Publish a message, returning a Task that that succeeds when the message is handled
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="publisher">
        /// The bus publisher
        /// </param>
        /// <param name="message">The message to publish</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task PublishAsync<TMessageType>(this IMessageBusPublisher<TMessageType> publisher, TMessageType message)
        {
            return publisher.PublishAsync(message, CancellationToken.None);
        }

        /// <summary>
        /// Publish a range of messages
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="publisher">
        /// The bus publisher
        /// </param>
        /// <param name="messages">The messages to publish</param>
        public static void PublishRange<TMessageType>(this IMessageBusPublisher<TMessageType> publisher, IEnumerable<TMessageType> messages)
        {
            // This call is not awaited, and that's the purpose. If it can finish synchronously, let it, otherwise, return control to the caller.
            publisher.PublishRangeAsync(messages);
        }

        /// <summary>
        /// Publish a message, returning a Task that that succeeds when the message is handled
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="publisher">
        /// The bus publisher
        /// </param>
        /// <param name="messages">The messages to publish</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task PublishRangeAsync<TMessageType>(this IMessageBusPublisher<TMessageType> publisher, IEnumerable<TMessageType> messages)
        {
            return Task.WhenAll(messages.Select(message => publisher.PublishAsync(message, CancellationToken.None)));
        }

        /// <summary>
        /// Publish a message, returning a Task that that succeeds when the message is handled
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="publisher">
        /// The bus publisher
        /// </param>
        /// <param name="messages">The messages to publish</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task PublishRangeAsync<TMessageType>(this IMessageBusPublisher<TMessageType> publisher, params TMessageType[] messages)
        {
            return Task.WhenAll(messages.Select(message => publisher.PublishAsync(message, CancellationToken.None)));
        }

        /// <summary>
        /// Publish a range of messages, returning a Task that that succeeds when the messages are handled
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <param name="publisher">
        /// The bus publisher
        /// </param>
        /// <param name="messages">The messages to publish</param>
        /// <param name="cancellationToken">A cancellation token that may cancel the handling of the message. </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task PublishRangeAsync<TMessageType>(
            this IMessageBusPublisher<TMessageType> publisher,
            IEnumerable<TMessageType> messages,
            CancellationToken cancellationToken)
        {
            return Task.WhenAll(messages.Select(message => publisher.PublishAsync(message, cancellationToken)));
        }
    }
}