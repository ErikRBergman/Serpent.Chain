// ReSharper disable once CheckNamespace

namespace Serpent.MessageHandlerChain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Decorators.AppendMany;

    /// <summary>
    ///     The append many decorator extensions
    /// </summary>
    public static class AppendManyExtensions
    {
        /// <summary>
        ///     Append a range of messages for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The chain message type
        /// </typeparam>
        /// <param name="messageHandlerChainBuilder">
        ///     The mch builder
        /// </param>
        /// <param name="configure">
        ///     Action called to configure the append many options
        /// </param>
        /// <returns>
        ///     A mch builder
        /// </returns>
        public static IMessageHandlerChainBuilder<TMessageType> AppendMany<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<IAppendManyDecoratorBuilder<TMessageType>> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var builder = new AppendManyDecoratorBuilder<TMessageType>();
            configure(builder);
            return messageHandlerChainBuilder.AddDecorator(builder.BuildDecorator());
        }

        /// <summary>
        ///     Append a range of messages for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <returns>A mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AppendMany<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<IEnumerable<TMessageType>>> messageSelector)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.AppendMany(c => c.Select(messageSelector));
        }

        /// <summary>
        ///     Append a range of messages for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AppendMany<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, CancellationToken, Task<IEnumerable<TMessageType>>> messageSelector)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.AppendMany(c => c.Select(messageSelector));
        }

        /// <summary>
        ///     Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> AppendMany<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, IEnumerable<TMessageType>> messageSelector)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.AddDecorator(
                innerMessageHandler =>
                    {
                        return (message, token) =>
                            {
#pragma warning disable CC0031 // Check for null before calling a delegate
                                var originalMessageTask = innerMessageHandler(message, token);
                                var newMessages = messageSelector(message);
                                if (newMessages == null)
                                {
                                    return originalMessageTask;
                                }

                                var newMessagesTask = Task.WhenAll(newMessages.Select(msg => innerMessageHandler(msg, token)));
#pragma warning restore CC0031 // Check for null before calling a delegate
                                return Task.WhenAll(originalMessageTask, newMessagesTask);
                            };
                    });
        }
    }
}