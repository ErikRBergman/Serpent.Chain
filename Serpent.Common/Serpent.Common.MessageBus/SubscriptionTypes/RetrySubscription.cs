namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Threading.Tasks;

    public class RetrySubscription<TMessageType> : BusSubscription<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly int maxNumberOfAttempts;

        private readonly TimeSpan retryDelay;

        public RetrySubscription(Func<TMessageType, Task> handlerFunc, int maxNumberOfAttempts, TimeSpan retryDelay)
        {
            this.handlerFunc = handlerFunc;
            this.maxNumberOfAttempts = maxNumberOfAttempts;
            if (this.maxNumberOfAttempts < 1)
            {
                throw new ArgumentException("Max number of attempts must be at least 1");
            }

            this.retryDelay = retryDelay;
        }

        public RetrySubscription(BusSubscription<TMessageType> innerSubscription, int maxNumberOfAttempts, TimeSpan retryDelay)
        {
            this.maxNumberOfAttempts = maxNumberOfAttempts;
            if (this.maxNumberOfAttempts < 1)
            {
                throw new ArgumentException("Max number of attempts must be at least 1");
            }

            this.retryDelay = retryDelay;
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
                    return; // success
                }
                catch (Exception exception)
                {
                    lastException = exception;
                }

                // await and then retry
                await Task.Delay(this.retryDelay).ConfigureAwait(false);
            }

            throw new Exception("Message handler failed " + this.maxNumberOfAttempts + " attempts.", lastException);
        }
    }
}