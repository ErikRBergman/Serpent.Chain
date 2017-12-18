namespace Serpent.Chain.Models
{
    using System;
    using System.Threading;

    /// <summary>
    /// Represents a failed attempt to handle a message
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public struct FailedMessageHandlingAttempt<TMessageType>
    {
        /// <summary>
        /// Gets the message
        /// </summary>
        public TMessageType Message { get; set; }

        /// <summary>
        /// Gets the attempt number
        /// </summary>
        public int AttemptNumber { get; set; }

        /// <summary>
        /// Gets the maximum number of attempts
        /// </summary>
        public int MaximumNumberOfAttemps { get; set; }

        /// <summary>
        /// Gets the delay before the next attempt
        /// </summary>
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// Gets the exception that was thrown
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets the cancellation token
        /// </summary>
        public CancellationToken CancellationToken { get; set; }
    }
}