﻿namespace Serpent.Chain.Decorators.Retry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Exceptions;

    internal class RetryDecorator<TMessageType> : ChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly int maxNumberOfAttempts;

        private readonly TimeSpan[] retryDelays;

        private readonly Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task<bool>> exceptionFunc;

        private readonly Func<TMessageType, int, int, Task> successFunc;

        private readonly int lastRetryDelay;

        public RetryDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, IRetryDecoratorBuilder<TMessageType> retryDecoratorBuilder)
        {
            this.handlerFunc = handlerFunc;
            this.maxNumberOfAttempts = retryDecoratorBuilder.MaximumNumberOfAttempts;
            if (this.maxNumberOfAttempts < 1)
            {
                throw new ArgumentException("Max number of attempts must be at least 1");
            }

            this.retryDelays = retryDecoratorBuilder.RetryDelays.ToArray();

            if (this.retryDelays.Length == 0)
            {
                throw new ArgumentException("At least one retry delay must be specified");
            }

            this.exceptionFunc = retryDecoratorBuilder.HandlerFailedFunc;
            this.successFunc = retryDecoratorBuilder.HandlerSucceededFunc;

            this.lastRetryDelay = this.retryDelays.Length - 1;
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            List<Exception> exceptions = null;

            var attempt = 0;

            for (; attempt < this.maxNumberOfAttempts; attempt++)
            {
                Exception lastException;
                try
                {
                    await this.handlerFunc(message, token).ConfigureAwait(false);

                    if (this.successFunc != null)
                    {
                        await this.successFunc(message, attempt + 1, this.maxNumberOfAttempts).ConfigureAwait(false);
                    }

                    return; // success
                }
                catch (Exception exception)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>(this.maxNumberOfAttempts);
                    }

                    exceptions.Add(exception);
                    lastException = exception;
                }

                var retryDelay = this.retryDelays[Math.Min(attempt, this.lastRetryDelay)];

                if (this.exceptionFunc != null && await this.exceptionFunc(message, lastException, attempt + 1, this.maxNumberOfAttempts, retryDelay, token).ConfigureAwait(false) == false)
                {
                    // ensure the attempt count is correct for the retry failed exception
                    attempt++;
                    break;
                }

                if (attempt != this.maxNumberOfAttempts - 1)
                {
                    // await and then retry
                    await Task.Delay(retryDelay, token).ConfigureAwait(false);
                }
            }

            throw new RetryFailedException("Message handler failed after " + attempt + " attempts. Errors: " + (exceptions != null ? string.Join(",", exceptions.Select((e, i) => (i + 1).ToString() + "." + e.Message)) : string.Empty), attempt, this.retryDelays, exceptions);
        }
    }
}