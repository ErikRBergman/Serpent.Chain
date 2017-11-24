namespace Serpent.Chain.Decorators.Semaphore
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The key semaphore type
    /// </summary>
    /// <typeparam name="TKeyType">The key type</typeparam>
    public class KeySemaphore<TKeyType>
    {
        private readonly ConcurrentDictionary<TKeyType, SemaphoreSlim> semaphores;

        /// <summary>
        /// Initializes a new instance of the key semaphore
        /// </summary>
        /// <param name="maxNumberOfConcurrentMessages">The maximum number of concurrent messages</param>
        public KeySemaphore(int maxNumberOfConcurrentMessages)
        {
            if (maxNumberOfConcurrentMessages < 1)
            {
                throw new ArgumentException("Maximum number of concurrent messages must not be less than 1", nameof(maxNumberOfConcurrentMessages));
            }

            this.MaxNumberOfConcurrentMessages = maxNumberOfConcurrentMessages;
            this.semaphores = new ConcurrentDictionary<TKeyType, SemaphoreSlim>();
        }

        /// <summary>
        /// Initializes a new instance of the key semaphore
        /// </summary>
        /// <param name="maxNumberOfConcurrentMessages">The maximum number of concurrent messages</param>
        /// <param name="equalityComparer">The equality comparer</param>
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

        /// <summary>
        /// The method called by the Semaphore decorator to execute a message handler
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="key">The key</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="handler">The message handler</param>
        /// <returns>A task</returns>
        public async Task ExecuteConcurrently<TMessageType>(TKeyType key, TMessageType message, CancellationToken cancellationToken, Func<TMessageType, CancellationToken, Task> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

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