// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System;
    using System.Collections.Generic;

    using Serpent.Chain.Decorators.Branch;

    /// <summary>
    ///     The branch decorator extensions
    /// </summary>
    public static class BranchExtensions
    {
        /// <summary>
        ///     Branches the message chain into multiple branches, where each has it's own handler or factory
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="firstBranch">The first branch</param>
        /// <param name="branches">The other branch(es)</param>
        public static void Branch<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Action<IChainBuilder<TMessageType>> firstBranch,
            params Action<IChainBuilder<TMessageType>>[] branches)
        {
            if (firstBranch == null)
            {
                throw new ArgumentNullException(nameof(firstBranch));
            }

            chainBuilder.Handle(
                services =>
                    {
                        var allBranches = new List<Action<IChainBuilder<TMessageType>>>
                        {
                            firstBranch
                        };

                        allBranches.AddRange(branches);

                        var handler = new BranchHandler<TMessageType>(allBranches);
                        services.BuilderNotifier.AddNotification(handler.ChainBuilt);
                        return handler.HandleMessageAsync;
                    });
        }
    }
}