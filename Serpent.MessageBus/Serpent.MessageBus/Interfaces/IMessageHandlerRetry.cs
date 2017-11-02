// ReSharper disable once CheckNamespace
namespace Serpent.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The IMessageHandlerRetry interface
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IMessageHandlerRetry<in TMessageType>
    {
        /// <summary>
        /// Called when a message handler has failed
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="exception">The exception</param>
        /// <param name="attemptNumber">The current attempt number</param>
        /// <param name="maxNumberOfAttemps">The maximum number of attempts</param>
        /// <param name="delay">The delay</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task</returns>
        Task HandleRetryAsync(TMessageType message, Exception exception, int attemptNumber, int maxNumberOfAttemps, TimeSpan delay, CancellationToken cancellationToken);

        /// <summary>
        /// Called when the retry decorator has executed a message handler successfully (even after the first attempt)
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="attemptNumber">The attempt number</param>
        /// <param name="maxNumberOfAttemps">The maximum number of attempts</param>
        /// <param name="delay">The delay</param>
        /// <returns>A task</returns>
        Task MessageHandledSuccessfullyAsync(TMessageType message, int attemptNumber, int maxNumberOfAttemps, TimeSpan delay);
    }
}