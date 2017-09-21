namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Threading.Tasks;

    public class RetrySubscription<TMessageType> : BusSubscription<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly int maxNumberOfAttempts;

        private readonly TimeSpan retryDelay;

        private readonly Func<TMessageType, Exception, int, int, Task> exceptionFunc;

        private readonly Func<TMessageType, Task> successFunc;

        public RetrySubscription(Func<TMessageType, Task> handlerFunc, int maxNumberOfAttempts, TimeSpan retryDelay, Func<TMessageType, Exception, int, int, Task> exceptionFunc = null, Func<TMessageType, Task> successFunc = null)
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

        public RetrySubscription(BusSubscription<TMessageType> innerSubscription, int maxNumberOfAttempts, TimeSpan retryDelay, Func<TMessageType, Exception, int, int, Task> exceptionFunc = null, Func<TMessageType, Task> successFunc = null)
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

        public override async Task HandleMessageAsync(TMessageType message)
        {
            Exception lastException = null;

            for (int i = 0; i < this.maxNumberOfAttempts; i++)
            {
                try
                {
                    await this.handlerFunc(message).ConfigureAwait(false);

                    if (this.successFunc != null)
                    {
                        await this.successFunc(message);
                    }

                    return; // success
                }
                catch (Exception exception)
                {
                    lastException = exception;
                }

                if (this.exceptionFunc != null)
                {
                    await this.exceptionFunc.Invoke(message, lastException, i + 1, this.maxNumberOfAttempts);
                }

                if (i != this.maxNumberOfAttempts - 1)
                {
                    // await and then retry
                    await Task.Delay(this.retryDelay).ConfigureAwait(false);
                }
            }

            throw new Exception("Message handler failed " + this.maxNumberOfAttempts + " attempts.", lastException);
        }
    }
}