// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using Serpent.Chain.Decorators.Skip;

    /// <summary>
    ///     Provides the .Skip() decorator extensions
    /// </summary>
    public static class SkipExtensions
    {
        /// <summary>
        ///     Skips a fixed number of messages before allowing all messages to pass through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="numberOfMessages">The number of messages to skip</param>
        /// <returns>A message handler chain builder</returns>
        public static IChainBuilder<TMessageType> Skip<TMessageType>(this IChainBuilder<TMessageType> chainBuilder, int numberOfMessages)
        {
            return chainBuilder.AddDecorator(currentHandler => new SkipDecorator<TMessageType>(currentHandler, numberOfMessages));
        }
    }
}