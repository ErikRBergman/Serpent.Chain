namespace Serpent.Chain.Decorators.Semaphore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SemaphoreWithKeyDecorator<TMessageType, TKeyType> : ChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, TKeyType> keySelector;

        private readonly KeySemaphore<TKeyType> keySemaphore;

        internal SemaphoreWithKeyDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, SemaphoreWithKeyDecoratorBuilder<TMessageType, TKeyType> builder)
        {
            this.handlerFunc = handlerFunc;
            this.keySelector = builder.KeySelectorValue;
            this.MaxNumberOfConcurrentMessages = builder.MaxNumberOfConcurrentMessagesValue;

            var equalityComparer = builder.EqualityComparerValue;

            var keySemaphoreValue = builder.KeySemaphoreValue;

            if (keySemaphoreValue == null)
            {
                if (equalityComparer == null)
                {
                    keySemaphoreValue = new KeySemaphore<TKeyType>(this.MaxNumberOfConcurrentMessages);
                }
                else
                {
                    keySemaphoreValue = new KeySemaphore<TKeyType>(this.MaxNumberOfConcurrentMessages, equalityComparer);
                }
            }

            this.keySemaphore = keySemaphoreValue;
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