// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The .Prepend() decorator extensions
    /// </summary>
    public static class CancellationExtensions
    {
        /// <summary>Throws a <see cref="T:System.OperationCanceledException"></see> if the token has had cancellation requested.</summary>
        /// <exception cref="T:System.OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource"></see> has been disposed.</exception>
        /// <typeparam name="TMessageType">The chain message type</typeparam>
        /// <param name="chainBuilder">The mch builder</param>
        /// <returns>The same mch builder</returns>
        public static IChainBuilder<TMessageType> ThrowIfCancellationRequested<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder)
        {
            return chainBuilder.AddDecorator(
                innerMessageHandler =>
                    {
                        return (message, token) =>
                            {
                                token.ThrowIfCancellationRequested();
                                return innerMessageHandler(message, token);
                            };
                    });
        }
    }
}