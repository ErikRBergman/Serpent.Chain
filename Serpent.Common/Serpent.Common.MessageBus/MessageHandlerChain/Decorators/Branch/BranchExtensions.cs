// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Branch;

    public static class BranchExtensions
    {
        /// <summary>
        /// Branches the message chain into multiple branches, where each has it's own handler or factory
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="branches">The branch(es)</param>
        /// <returns>A subscription</returns>
        public static IMessageBusSubscription Branch<TMessageType>(
        this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
        params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches)
        {
            return messageHandlerChainBuilder.Handler(new BranchHandler<TMessageType>(branches).HandleMessageAsync);
        }
    }
}