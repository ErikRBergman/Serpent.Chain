namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class SemaphoreDecorator<TMessageType> : MessageHandlerDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly SemaphoreSlim semaphore;

        public SemaphoreDecorator(Func<TMessageType, Task> handlerFunc, int maxNumberOfConcurrentMessages)
        {
            this.handlerFunc = handlerFunc;
            this.MaxNumberOfConcurrentMessages = maxNumberOfConcurrentMessages;
            this.semaphore = new SemaphoreSlim(maxNumberOfConcurrentMessages);
        }

        public SemaphoreDecorator(MessageHandlerDecorator<TMessageType> innerSubscription, int maxNumberOfConcurrentMessages)
        {
            this.MaxNumberOfConcurrentMessages = maxNumberOfConcurrentMessages;
            this.handlerFunc = innerSubscription.HandleMessageAsync;
            this.semaphore = new SemaphoreSlim(maxNumberOfConcurrentMessages);
        }

        private int MaxNumberOfConcurrentMessages { get; }

        public override async Task HandleMessageAsync(TMessageType message)
        {
            await this.semaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                await this.handlerFunc(message).ConfigureAwait(false);
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}