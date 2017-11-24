// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.Branch;

    /// <summary>
    /// The branch out extensions
    /// </summary>
    public static class BranchOutExtensions
    {
        /// <summary>
        /// Branch Out one or more parallel branches, running in parallel with the main message handler
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="branches">The branches</param>
        /// <returns>The message handler chain builder</returns>
        public static IChainBuilder<TMessageType> BranchOut<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            params Action<IChainBuilder<TMessageType>>[] branches)
        {
            return chainBuilder.Decorate(
                (innerHandler, services) =>
                    {
                        var handler = new BranchHandler<TMessageType>(branches);
                        services.BuilderNotifier.AddNotification(handler.ChainBuilt);
#pragma warning disable CC0031 // Check for null before calling a delegate
                        return (message, token) => Task.WhenAll(handler.HandleMessageAsync(message, token), innerHandler(message, token));
#pragma warning restore CC0031 // Check for null before calling a delegate
                    });
        }
    }
}