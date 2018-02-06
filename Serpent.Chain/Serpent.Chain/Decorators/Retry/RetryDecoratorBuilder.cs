namespace Serpent.Chain.Decorators.Retry
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Models;

    internal class RetryDecoratorBuilder<TMessageType> : IRetryDecoratorBuilder<TMessageType>
    {
        private readonly List<Func<FailedMessageHandlingAttempt<TMessageType>, bool>> wherePredicates = new List<Func<FailedMessageHandlingAttempt<TMessageType>, bool>>();

        public int MaximumNumberOfAttempts { get; set; }

        public IEnumerable<TimeSpan> RetryDelays { get; set; }

        public Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task<bool>> HandlerFailedFunc { get; set; }

        public Func<TMessageType, int, int, Task> HandlerSucceededFunc { get; set; }

        public IReadOnlyCollection<Func<FailedMessageHandlingAttempt<TMessageType>, bool>> WherePredicates => this.wherePredicates;

        public IRetryDecoratorBuilder<TMessageType> Where(Func<FailedMessageHandlingAttempt<TMessageType>, bool> predicate)
        {
            this.wherePredicates.Add(predicate);
            return this;
        }
    }
}