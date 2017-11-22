// ReSharper disable once CheckNamespace
namespace Serpent.MessageHandlerChain
{
    using System;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Decorators.Branch;

    /// <summary>
    /// The branch out extensions
    /// </summary>
    public static class BranchOutExtensions
    {
        /// <summary>
        /// Branch Out one or more parallel branches, running in parallel with the main message handler
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="branches">The branches</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> BranchOut<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches)
        {
            return messageHandlerChainBuilder.Decorate(
                (innerHandler, services) =>
                    {
                        var handler = new BranchHandler<TMessageType>(branches);
                        services.BuildNotification.AddNotification(handler.MessageHandlerChainBuilt);
#pragma warning disable CC0031 // Check for null before calling a delegate
                        return (message, token) => Task.WhenAll(handler.HandleMessageAsync(message, token), innerHandler(message, token));
#pragma warning restore CC0031 // Check for null before calling a delegate
                    });
        }
    }
}