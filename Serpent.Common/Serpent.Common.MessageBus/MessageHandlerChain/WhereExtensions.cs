// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain;

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
                    return async message =>
                        {
                            if (await asyncPredicate(message))
                            {
                                await innerMessageHandler(message);
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

            return messageHandlerChainBuilder.Add(innerMessageHandler => message =>
                {
                    if (predicate(message))
                    {
                        return innerMessageHandler(message);
                    }

                    return Task.CompletedTask;
                });
        }
    }
}