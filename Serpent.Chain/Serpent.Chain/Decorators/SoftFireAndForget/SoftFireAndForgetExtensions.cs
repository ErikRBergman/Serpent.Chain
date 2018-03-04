// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using Serpent.Chain.Decorators.SoftFireAndForget;

    /// <summary>
    /// Provides extensions for the .SoftFireAndForget decorator
    /// </summary>
    public static class SoftFireAndForgetExtensions
    {
        /// <summary>
        /// Drops the the feedback chain, making the message handler chain fire and forget unless the message handler chain returns synchronously
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <returns>A message handler chain builder</returns>
        public static IChainBuilder<TMessageType> SoftFireAndForget<TMessageType>(this IChainBuilder<TMessageType> chainBuilder)
        {
            return chainBuilder.AddDecorator(nextHandler => new SoftFireAndForgetDecorator<TMessageType>(nextHandler).HandleMessageAsync);
        }
    }
}