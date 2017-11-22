namespace Serpent.MessageHandlerChain.Decorators.Retry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Exceptions;

    internal class RetryDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly int maxNumberOfAttempts;

        private readonly TimeSpan retryDelay;

        private readonly Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task> exceptionFunc;

        private readonly Func<TMessageType, int, int, TimeSpan, Task> successFunc;

        public RetryDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, IRetryDecoratorBuilder<TMessageType> retryDecoratorBuilder)
        {
            this.handlerFunc = handlerFunc;
            this.maxNumberOfAttempts = retryDecoratorBuilder.MaximumNumberOfAttempts;
            if (this.maxNumberOfAttempts < 1)
            {
                throw new ArgumentException("Max number of attempts must be at least 1");
            }

            this.retryDelay = retryDecoratorBuilder.RetryDelay;
            this.exceptionFunc = retryDecoratorBuilder.HandlerFailedFunc;
            this.successFunc = retryDecoratorBuilder.HandlerSucceededFunc;
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            List<Exception> exceptions = null;

            for (int i = 0; i < this.maxNumberOfAttempts; i++)
            {
                Exception lastException;
                try
                {
                    await this.handlerFunc(message, token).ConfigureAwait(false);

                    if (this.successFunc != null)
                    {
                        await this.successFunc(message, i + 1, this.maxNumberOfAttempts, this.retryDelay).ConfigureAwait(false);
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

                if (this.exceptionFunc != null)
                {
                    await this.exceptionFunc.Invoke(message, lastException, i + 1, this.maxNumberOfAttempts, this.retryDelay, token).ConfigureAwait(false);
                }

                if (i != this.maxNumberOfAttempts - 1)
                {
                    // await and then retry
                    await Task.Delay(this.retryDelay, token).ConfigureAwait(false);
                }
            }

            throw new RetryFailedException("Message handler failed after " + this.maxNumberOfAttempts + " attempts. Errors: " + (exceptions != null ? string.Join(",", exceptions.Select((e, i) => (i + 1).ToString() + "." + e.Message)) : string.Empty), this.maxNumberOfAttempts, this.retryDelay, exceptions);
        }
    }
}