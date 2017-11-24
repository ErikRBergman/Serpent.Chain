// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    /// <summary>
    /// The cast decorator extensions
    /// </summary>
    public static class CastExtensions
    {
        /// <summary>
        /// Cast the message to a new type, returning a message handler chain builder of the new type
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TNewType">The new message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <returns>The message handler chain of the new type</returns>
        public static IChainBuilder<TNewType> Cast<TMessageType, TNewType>(
            this IChainBuilder<TMessageType> chainBuilder)
            where TNewType : class
        {
            return chainBuilder.Select(m => m as TNewType);
        }
    }
}