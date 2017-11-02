namespace Serpent.MessageBus.MessageHandlerChain.Decorators.Semaphore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SemaphoreWithKeyDecorator<TMessageType, TKeyType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, TKeyType> keySelector;

        private readonly KeySemaphore<TKeyType> keySemaphore;

        public SemaphoreWithKeyDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, TKeyType> keySelector, int maxNumberOfConcurrentMessages)
        {
            this.handlerFunc = handlerFunc;
            this.keySelector = keySelector;
            this.MaxNumberOfConcurrentMessages = maxNumberOfConcurrentMessages;
            this.keySemaphore = new KeySemaphore<TKeyType>(maxNumberOfConcurrentMessages);
        }

        public SemaphoreWithKeyDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, TKeyType> keySelector, KeySemaphore<TKeyType> keySemaphore)
        {
            this.keySemaphore = keySemaphore ?? throw new ArgumentNullException(nameof(keySemaphore));

            this.handlerFunc = handlerFunc;
            this.keySelector = keySelector;
            this.MaxNumberOfConcurrentMessages = keySemaphore.MaxNumberOfConcurrentMessages;
        }

        private int MaxNumberOfConcurrentMessages { get; }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken)
        {
            await this.keySemaphore.ExecuteConcurrently(
                this.keySelector(message), 
                message, 
                cancellationToken,
                (msg, token) => this.handlerFunc(message, token)).ConfigureAwait(false);
        }
    }
}