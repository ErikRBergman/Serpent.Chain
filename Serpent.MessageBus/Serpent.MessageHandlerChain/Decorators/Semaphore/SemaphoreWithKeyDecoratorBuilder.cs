namespace Serpent.MessageHandlerChain.Decorators.Semaphore
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain;

    /// <summary>
    /// Provides configuration builder for the semaphore with keys
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    /// <typeparam name="TKeyType">The key type</typeparam>
    public class SemaphoreWithKeyDecoratorBuilder<TMessageType, TKeyType>
    {
        internal IEqualityComparer<TKeyType> EqualityComparerValue { get; private set; }

        internal Func<TMessageType, TKeyType> KeySelectorValue { get; private set; }

        internal KeySemaphore<TKeyType> KeySemaphoreValue { get; private set; }

        internal int MaxNumberOfConcurrentMessagesValue { get; private set; } = 1;

        /// <summary>
        ///  Sets the equality comparer
        /// </summary>
        /// <param name="equalityComparer">The equality comparer</param>
        /// <returns>A builder</returns>
        public SemaphoreWithKeyDecoratorBuilder<TMessageType, TKeyType> EqualityComparer(IEqualityComparer<TKeyType> equalityComparer)
        {
            this.EqualityComparerValue = equalityComparer;
            return this;
        }

        /// <summary>
        ///     Sets the key selector
        /// </summary>
        /// <param name="keySelector">The key selector</param>
        /// <returns>A builder</returns>
        public SemaphoreWithKeyDecoratorBuilder<TMessageType, TKeyType> KeySelector(Func<TMessageType, TKeyType> keySelector)
        {
            this.KeySelectorValue = keySelector;
            return this;
        }

        /// <summary>
        ///     Sets the key semaphore
        /// </summary>
        /// <param name="keySemaphore">A key semaphore</param>
        /// <returns>A SemaphoreWithKeyDecoratorBuilder</returns>
        public SemaphoreWithKeyDecoratorBuilder<TMessageType, TKeyType> KeySemaphore(KeySemaphore<TKeyType> keySemaphore)
        {
            this.KeySemaphoreValue = keySemaphore;
            return this;
        }

        /// <summary>
        ///     Limits the message handler chain to X concurrent messages being handled.
        ///     This does not add concurrency but limits it.
        /// </summary>
        /// <param name="maxNumberOfConcurrentMessages">The maximum concurrency level</param>
        /// <returns>A builder</returns>
        public SemaphoreWithKeyDecoratorBuilder<TMessageType, TKeyType> MaxNumberOfConcurrentMessages(int maxNumberOfConcurrentMessages)
        {
            this.MaxNumberOfConcurrentMessagesValue = maxNumberOfConcurrentMessages;
            return this;
        }

        internal MessageHandlerChainDecorator<TMessageType> Build(Func<TMessageType, CancellationToken, Task> currentHandler)
        {
            return new SemaphoreWithKeyDecorator<TMessageType, TKeyType>(currentHandler, this);
        }
    }
}