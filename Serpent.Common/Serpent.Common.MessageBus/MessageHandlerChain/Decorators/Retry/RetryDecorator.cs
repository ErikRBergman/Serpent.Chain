namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class RetryDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly int maxNumberOfAttempts;

        private readonly TimeSpan retryDelay;

        private readonly Func<TMessageType, Exception, int, int, Task> exceptionFunc;

        private readonly Func<TMessageType, Task> successFunc;

        public RetryDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, int maxNumberOfAttempts, TimeSpan retryDelay, Func<TMessageType, Exception, int, int, Task> exceptionFunc = null, Func<TMessageType, Task> successFunc = null)
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

        public RetryDecorator(MessageHandlerChainDecorator<TMessageType> innerSubscription, int maxNumberOfAttempts, TimeSpan retryDelay, Func<TMessageType, Exception, int, int, Task> exceptionFunc = null, Func<TMessageType, Task> successFunc = null)
        {
            this.maxNumberOfAttempts = maxNumberOfAttempts;
            if (this.maxNumberOfAttempts < 1)
            {
                throw new ArgumentException("Max number of attempts must be at least 1");
            }

            this.retryDelay = retryDelay;
            this.exceptionFunc = exceptionFunc;
            this.successFunc = successFunc;
            this.handlerFunc = innerSubscription.HandleMessageAsync;
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            Exception lastException = null;

            for (int i = 0; i < this.maxNumberOfAttempts; i++)
            {
                try
                {
                    await this.handlerFunc(message, token).ConfigureAwait(false);

                    if (this.successFunc != null)
                    {
                        await this.successFunc(message).ConfigureAwait(false);
                    }

                    return; // success
                }
                catch (Exception exception)
                {
                    lastException = exception;
                }

                if (this.exceptionFunc != null)
                {
                    await this.exceptionFunc.Invoke(message, lastException, i + 1, this.maxNumberOfAttempts).ConfigureAwait(false);
                }

                if (i != this.maxNumberOfAttempts - 1)
                {
                    // await and then retry
                    await Task.Delay(this.retryDelay, token).ConfigureAwait(false);
                }
            }

            throw new Exception("Message handler failed " + this.maxNumberOfAttempts + " attempts.", lastException);
        }
    }
}