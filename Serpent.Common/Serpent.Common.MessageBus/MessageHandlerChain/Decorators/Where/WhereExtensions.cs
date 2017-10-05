// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    public static class WhereExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> Where<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> asyncPredicate)
        {
            if (asyncPredicate == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(innerMessageHandler =>
                {
                    return async (message, token) =>
                        {
                            if (await asyncPredicate(message).ConfigureAwait(false))
                            {
                                await innerMessageHandler(message, token).ConfigureAwait(false);
                            }
                        };
                });
        }

        public static IMessageHandlerChainBuilder<TMessageType> Where<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> predicate)
        {
            if (predicate == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(innerMessageHandler => (message, token) =>
                {
                    if (predicate(message))
                    {
                        return innerMessageHandler(message, token);
                    }

                    return Task.CompletedTask;
                });
        }
    }
}