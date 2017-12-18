// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System.Threading.Tasks;

    using Serpent.Chain.Models;

    /// <summary>
    /// The IMessageHandlerRetry interface
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IMessageHandlerRetry<TMessageType>
    {
        /// <summary>
        /// Called when a message handler has failed
        /// </summary>
        /// <param name="attemptInformation">
        /// Failed attempt information
        /// </param>
        /// <returns>
        /// A task
        /// </returns>
        Task<bool> HandleRetryAsync(FailedMessageHandlingAttempt<TMessageType> attemptInformation);

        /// <summary>
        /// Called when the retry decorator has executed a message handler successfully (even after the first attempt)
        /// </summary>
        /// <param name="attemptInformation">
        /// The attempt that succeeded
        /// </param>
        /// <returns>
        /// A task
        /// </returns>
        Task MessageHandledSuccessfullyAsync(MessageHandlingAttempt<TMessageType> attemptInformation);
    }
}