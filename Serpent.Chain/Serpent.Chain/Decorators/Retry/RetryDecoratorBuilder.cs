namespace Serpent.Chain.Decorators.Retry
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Models;

    internal class RetryDecoratorBuilder<TMessageType> : IRetryDecoratorBuilder<TMessageType>
    {
        public int MaximumNumberOfAttempts { get; set; }

        public IEnumerable<TimeSpan> RetryDelays { get; set; }

        public Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task<bool>> HandlerFailedFunc { get; set; }

        public Func<TMessageType, int, int, Task> HandlerSucceededFunc { get; set; }

        public Func<FailedMessageHandlingAttempt<TMessageType>, bool> WherePredicate { get; set; }
    }
}