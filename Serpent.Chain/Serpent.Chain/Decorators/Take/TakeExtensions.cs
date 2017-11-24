// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using Serpent.Chain.Decorators.Take;

    /// <summary>
    /// Provides the .Take() decorator extensions for message handler chain builder 
    /// </summary>
    public static class TakeExtensions
    {
        /// <summary>
        /// Only takes a specified number of messages before unsubscribing
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="numberOfMessages">The number of messages to take</param>
        /// <returns>A message handler chain builder</returns>
        public static IChainBuilder<TMessageType> Take<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            int numberOfMessages)
        {
            return chainBuilder.AddDecorator((currentHandler, services) => new TakeDecorator<TMessageType>(currentHandler, numberOfMessages, services.BuilderNotifier));
        }
    }
}