namespace Serpent.MessageBus.Models
{
    using System;

    /// <summary>
    /// Represents an attempt to handle a message
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public struct MessageHandlingAttempt<TMessageType>
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
        /// Gets the delay between attempts
        /// </summary>
        public TimeSpan Delay { get; set; }
    }
}