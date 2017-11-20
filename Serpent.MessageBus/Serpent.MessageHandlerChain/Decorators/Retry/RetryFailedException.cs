// ReSharper disable once CheckNamespace
namespace Serpent.MessageHandlerChain.Exceptions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The retry failed exception. This exception is thrown when all retry attempts fail.
    /// </summary>
    public class RetryFailedException : Exception
    {
        /// <summary>
        /// Creates a new instance of the retry failed exception
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="numberOfAttempts">The number of attempts done</param>
        /// <param name="delay">The delay between the attempts</param>
        /// <param name="exceptions">All thrown exceptions</param>
        public RetryFailedException(string message, int numberOfAttempts, TimeSpan delay, IReadOnlyCollection<Exception> exceptions)
            : base(message)
        {
            this.NumberOfAttempts = numberOfAttempts;
            this.Delay = delay;
            this.Exceptions = exceptions;
        }

        /// <summary>
        /// The number of attempts
        /// </summary>
        public int NumberOfAttempts { get; }

        /// <summary>
        /// The delay between attempts
        /// </summary>
        public TimeSpan Delay { get; }

        /// <summary>
        /// The exceptions
        /// </summary>
        public IReadOnlyCollection<Exception> Exceptions { get; }
    }
}