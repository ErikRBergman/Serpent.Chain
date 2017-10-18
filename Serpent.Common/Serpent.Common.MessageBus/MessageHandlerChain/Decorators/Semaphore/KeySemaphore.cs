namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Semaphore
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class KeySemaphore<TKeyType>
    {
        private readonly ConcurrentDictionary<TKeyType, SemaphoreSlim> semaphores;

        public KeySemaphore(int maxNumberOfConcurrentMessages)
        {
            if (maxNumberOfConcurrentMessages < 1)
            {
                throw new ArgumentException("Maximum number of concurrent messages must not be less than 1", nameof(maxNumberOfConcurrentMessages));
            }

            this.MaxNumberOfConcurrentMessages = maxNumberOfConcurrentMessages;
            this.semaphores = new ConcurrentDictionary<TKeyType, SemaphoreSlim>();
        }

        public KeySemaphore(int maxNumberOfConcurrentMessages, IEqualityComparer<TKeyType> equalityComparer)
        {
            if (maxNumberOfConcurrentMessages < 1)
            {
                throw new ArgumentException("Maximum number of concurrent messages must not be less than 1", nameof(maxNumberOfConcurrentMessages));
            }

            this.MaxNumberOfConcurrentMessages = maxNumberOfConcurrentMessages;
            this.semaphores = new ConcurrentDictionary<TKeyType, SemaphoreSlim>(equalityComparer);
        }

        /// <summary>
        /// The maximum number of concurrent messages with the specified key
        /// </summary>
        public int MaxNumberOfConcurrentMessages { get; }

        public async Task ExecuteConcurrently<TMessageType>(TKeyType key, TMessageType message, CancellationToken cancellationToken, Func<TMessageType, CancellationToken, Task> handler)
        {
            var semaphore = this.semaphores.GetOrAdd(key, _ => new SemaphoreSlim(this.MaxNumberOfConcurrentMessages));

            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                await handler(message, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}