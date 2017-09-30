// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    public static class AppendExtensions
    {
        /// <summary>
        ///     Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Append<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<TMessageType>> messageSelector)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                innerMessageHandler =>
                    {
                        return async message =>
                            {
                                var chainedMessageTask = innerMessageHandler(message);
                                await Task.WhenAll(chainedMessageTask, InnerMessageHandlerAsync(innerMessageHandler, messageSelector, message));
                            };
                    });
        }

        /// <summary>
        ///     Append a second message for each message passed through
        /// </summary>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The mch builder</param>
        /// <param name="predicate">Append only if this predicate returns true</param>
        /// <param name="messageSelector">The function used to create the new message</param>
        /// <param name="isRecursive">Set to true to run the new message through this Append</param>
        /// <returns>The same mch builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Append<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> predicate,
            Func<TMessageType, Task<TMessageType>> messageSelector,
            bool isRecursive = false)
        {
            if (messageSelector == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(innerMessageHandler => message => Task.WhenAll(
                innerMessageHandler(message),
                AppendIfAsync(innerMessageHandler, predicate, messageSelector, message, isRecursive)));
        }


    /// <summary>
    ///     Append a second message for each message passed through
    /// </summary>
    /// <typeparam name="TMessageType">The chain message type</typeparam>
    /// <param name="messageHandlerChainBuilder">The mch builder</param>
    /// <param name="messageSelector">The function used to create the new message</param>
    /// <returns>The same mch builder</returns>
    public static IMessageHandlerChainBuilder<TMessageType> Append<TMessageType>(
        this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
        Func<TMessageType, TMessageType> messageSelector)
    {
        if (messageSelector == null)
        {
            return messageHandlerChainBuilder;
        }

        return messageHandlerChainBuilder.Add(
            innerMessageHandler =>
                {
                    return async message =>
                        {
                            var originalMessageTask = innerMessageHandler(message);
                            var newMessageTask = innerMessageHandler(messageSelector(message));
                            await Task.WhenAll(originalMessageTask, newMessageTask).ConfigureAwait(false);
                        };
                });
    }

    /// <summary>
    ///     Append a second message for each message passed through
    /// </summary>
    /// <typeparam name="TMessageType">The chain message type</typeparam>
    /// <param name="messageHandlerChainBuilder">The mch builder</param>
    /// <param name="predicate">Append only if this predicate returns true</param>
    /// <param name="messageSelector">The function used to create the new message</param>
    /// <returns>The same mch builder</returns>
    public static IMessageHandlerChainBuilder<TMessageType> Append<TMessageType>(
        this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
        Func<TMessageType, bool> predicate,
        Func<TMessageType, TMessageType> messageSelector,
        bool isRecursive = false)
    {
        if (messageSelector == null)
        {
            return messageHandlerChainBuilder;
        }

        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate), "Predicate may not be null");
        }

        return messageHandlerChainBuilder.Add(
            innerMessageHandler =>
                message =>
                    Task.WhenAll(innerMessageHandler(message), AppendIfAsync(predicate, messageSelector, innerMessageHandler, message, isRecursive)));
    }

    private static Task AppendIfAsync<TMessageType>(Func<TMessageType, bool> predicate, Func<TMessageType, TMessageType> messageSelector, Func<TMessageType, Task> innerMessageHandler, TMessageType message, bool isRecursive)
    {
        if (predicate(message) == false)
        {
            return Task.CompletedTask;
        }

        var newMessage = messageSelector(message);
        var newMessageTask = innerMessageHandler(newMessage);

        if (isRecursive)
        {
            return Task.WhenAll(newMessageTask, AppendIfAsync(predicate, messageSelector, innerMessageHandler, newMessage, true));
        }

        return newMessageTask;
    }

    private static async Task AppendIfAsync<TMessageType>(
        Func<TMessageType, Task> messageHandler,
        Func<TMessageType, Task<bool>> predicate,
        Func<TMessageType, Task<TMessageType>> messageSelector,
        TMessageType originalMessage,
        bool isRecursive)
    {
        if (await predicate(originalMessage).ConfigureAwait(false))
        {
            var newMessage = await messageSelector(originalMessage).ConfigureAwait(false);
            if (isRecursive)
            {
                await Task.WhenAll(messageHandler(newMessage), AppendIfAsync(messageHandler, predicate, messageSelector, newMessage, true));
            }
            else
            {
                await messageHandler(newMessage).ConfigureAwait(false);
            }
        }
    }

    private static async Task InnerMessageHandlerAsync<TMessageType>(
        Func<TMessageType, Task> messageHandler,
        Func<TMessageType, Task<TMessageType>> messageSelector,
        TMessageType originalMessage)
    {
        var newMessage = await messageSelector(originalMessage).ConfigureAwait(false);
        await messageHandler(newMessage).ConfigureAwait(false);
    }
}
}