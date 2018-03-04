// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using Serpent.Chain.Decorators.FireAndForget;

    /// <summary>
    /// The fire and forget decorator extensions
    /// </summary>
    public static class FireAndForgetExtensions
    {
        /// <summary>
        /// Execute each message handled on a new Task
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <returns>The message handler chain builder</returns>
        public static IChainBuilder<TMessageType> FireAndForget<TMessageType>(this IChainBuilder<TMessageType> chainBuilder)
        {
            return chainBuilder.AddDecorator(nextHandler => new FireAndForgetDecorator<TMessageType>(nextHandler).HandleMessageAsync);
        }
    }
}