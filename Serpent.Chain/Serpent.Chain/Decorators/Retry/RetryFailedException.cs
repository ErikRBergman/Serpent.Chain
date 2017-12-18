// ReSharper disable once CheckNamespace
namespace Serpent.Chain.Exceptions
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
        /// <param name="delays">The delay(s) between the attempts. The first delay is after the first attempt, the second after the second and so on. The last delay is used for all other.</param>
        /// <param name="exceptions">All thrown exceptions</param>
        public RetryFailedException(string message, int numberOfAttempts, IReadOnlyCollection<TimeSpan> delays, IReadOnlyCollection<Exception> exceptions)
            : base(message)
        {
            this.NumberOfAttempts = numberOfAttempts;
            this.Delays = delays;
            this.Exceptions = exceptions;
        }

        /// <summary>
        /// The number of attempts
        /// </summary>
        public int NumberOfAttempts { get; }

        /// <summary>
        /// The delay between attempts
        /// </summary>
        public IReadOnlyCollection<TimeSpan> Delays { get; }

        /// <summary>
        /// The exceptions
        /// </summary>
        public IReadOnlyCollection<Exception> Exceptions { get; }
    }
}