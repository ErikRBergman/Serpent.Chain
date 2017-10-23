// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Branch;

    /// <summary>
    ///     The branch decorator extensions
    /// </summary>
    public static class BranchExtensions
    {
        /// <summary>
        ///     Branches the message chain into multiple branches, where each has it's own handler or factory
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="firstBranch">The first branch</param>
        /// <param name="branches">The other branch(es)</param>
        /// <returns>A subscription</returns>
        public static IMessageBusSubscription Branch<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<IMessageHandlerChainBuilder<TMessageType>> firstBranch,
            params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches)
        {
            if (firstBranch == null)
            {
                throw new ArgumentNullException(nameof(firstBranch));
            }

            var allBranches = new List<Action<IMessageHandlerChainBuilder<TMessageType>>>
                                  {
                                      firstBranch
                                  };
            allBranches.AddRange(branches);

            var handler = new BranchHandler<TMessageType>(allBranches);
            var subscription = messageHandlerChainBuilder.Handler(handler.HandleMessageAsync);
            handler.SetSubscription(subscription);

            return subscription;
        }
    }
}