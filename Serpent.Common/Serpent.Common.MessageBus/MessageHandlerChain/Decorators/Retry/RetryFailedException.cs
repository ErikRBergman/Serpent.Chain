// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus.Exceptions
{
    using System;
    using System.Collections.Generic;

    public class RetryFailedException : Exception
    {
        public RetryFailedException(string message, int numberOfAttempts, TimeSpan delay, IReadOnlyCollection<Exception> exceptions)
            : base(message)
        {
            this.NumberOfAttempts = numberOfAttempts;
            this.Delay = delay;
            this.Exceptions = exceptions;
        }

        public int NumberOfAttempts { get; }

        public TimeSpan Delay { get; }

        public IReadOnlyCollection<Exception> Exceptions { get; }
    }
}