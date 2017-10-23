// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Filter;

    /// <summary>
    /// The filter extensions
    /// </summary>
    public static class FilterExtensions
    {
        /// <summary>
        /// Filter messages
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="beforeInvoke">Executed before the message is invoked (returning false to remove the message)</param>
        /// <param name="afterInvoke">Executed after the handler is executed successfully</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Filter<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> beforeInvoke = null,
            Func<TMessageType, Task> afterInvoke = null)
        {
            if (beforeInvoke == null && afterInvoke == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                currentHandler => new FilterDecorator<TMessageType>(currentHandler, (msg, _) => beforeInvoke?.Invoke(msg), (msg, _) => afterInvoke?.Invoke(msg)));
        }

        /// <summary>
        /// Filter messages
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="beforeInvoke">Executed before the message is invoked (returning false to remove the message)</param>
        /// <param name="afterInvoke">Executed after the handler is executed successfully</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Filter<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, CancellationToken, Task<bool>> beforeInvoke = null,
            Func<TMessageType, CancellationToken, Task> afterInvoke = null)
        {
            if (beforeInvoke == null && afterInvoke == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(currentHandler => new FilterDecorator<TMessageType>(currentHandler, beforeInvoke, afterInvoke));
        }

        /// <summary>
        /// Filter messages
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="beforeInvoke">Executed before the message is invoked (returning false to remove the message)</param>
        /// <param name="afterInvoke">Executed after the handler is executed successfully</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Filter<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> beforeInvoke = null,
            Action<TMessageType> afterInvoke = null)
        {
            if (beforeInvoke == null && afterInvoke == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                currentHandler => new FilterDecorator<TMessageType>(
                    currentHandler,
                    (message, token) => Task.FromResult(beforeInvoke?.Invoke(message) ?? true),
                    (message, token) =>
                        {
                            afterInvoke?.Invoke(message);
                            return Task.CompletedTask;
                        }));
        }

        /// <summary>
        /// Filter messages
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="beforeInvoke">Executed before the message is invoked (returning false to remove the message)</param>
        /// <param name="afterInvoke">Executed after the handler is executed successfully</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Filter<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<TMessageType> beforeInvoke = null,
            Action<TMessageType> afterInvoke = null)
        {
            if (beforeInvoke == null && afterInvoke == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(
                currentHandler => new FilterDecorator<TMessageType>(
                    currentHandler,
                    (message, token) =>
                        {
                            beforeInvoke?.Invoke(message);
                            return Task.FromResult(true);
                        },
                    (message, token) =>
                        {
                            afterInvoke?.Invoke(message);
                            return Task.CompletedTask;
                        }));
        }
    }
}