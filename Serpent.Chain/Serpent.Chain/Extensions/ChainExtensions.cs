// ReSharper disable CheckNamespace
namespace Serpent.Chain
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides message handler chain extensions
    /// </summary>
    public static class ChainExtensions
    {
        /// <summary>
        /// Handle a message on the message handler chain
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chain">The message handler Chain</param>
        /// <param name="message">The message to handle</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A task that completes when the message is handled</returns>
        public static Task HandleMessageAsync<TMessageType>(this IChain<TMessageType> chain, TMessageType message, CancellationToken cancellationToken)
        {
            return chain.ChainFunc(message, cancellationToken);
        }

        /// <summary>
        /// Handle a message on the message handler chain
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chain">The message handler Chain</param>
        /// <param name="message">The message to handle</param>
        /// <returns>A task that completes when the message is handled</returns>
        public static Task HandleMessageAsync<TMessageType>(this IChain<TMessageType> chain, TMessageType message)
        {
            return chain.ChainFunc(message, CancellationToken.None);
        }
    }
}