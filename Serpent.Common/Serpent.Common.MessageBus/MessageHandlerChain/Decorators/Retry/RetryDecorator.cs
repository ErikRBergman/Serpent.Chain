namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Exceptions;

    public class RetryDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly int maxNumberOfAttempts;

        private readonly TimeSpan retryDelay;

        private readonly Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task> exceptionFunc;

        private readonly Func<TMessageType, int, int, TimeSpan, Task> successFunc;

        public RetryDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, int maxNumberOfAttempts, TimeSpan retryDelay, Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task> exceptionFunc = null, Func<TMessageType, int, int, TimeSpan, Task> successFunc = null)
        {
            this.handlerFunc = handlerFunc;
            this.maxNumberOfAttempts = maxNumberOfAttempts;
            if (this.maxNumberOfAttempts < 1)
            {
                throw new ArgumentException("Max number of attempts must be at least 1");
            }

            this.retryDelay = retryDelay;
            this.exceptionFunc = exceptionFunc;
            this.successFunc = successFunc;
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            Exception lastException;

            List<Exception> exceptions = null;

            for (int i = 0; i < this.maxNumberOfAttempts; i++)
            {
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

            throw new RetryFailedException("Message handler failed " + this.maxNumberOfAttempts + " attempts.", this.maxNumberOfAttempts, this.retryDelay, exceptions);
        }
    }
}