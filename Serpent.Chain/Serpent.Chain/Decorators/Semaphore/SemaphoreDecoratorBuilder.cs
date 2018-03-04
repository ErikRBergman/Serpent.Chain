// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.Semaphore;

    /// <summary>
    ///     Provides a builder for the semaphore decorator
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public class SemaphoreDecoratorBuilder<TMessageType>
    {
        private Func<Func<TMessageType, CancellationToken, Task>, ChainDecorator<TMessageType>> buildFunc;

        private int maxNumberOfConcurrentMessages = 1;

        /// <summary>
        ///     asda
        /// </summary>
        public SemaphoreDecoratorBuilder()
        {
            this.buildFunc = this.BuildInternal;
        }

        /// <summary>
        ///     Sets an equality comparer
        /// </summary>
        /// <typeparam name="TKeyType">The key types</typeparam>
        /// <param name="equalityComparer">
        ///     The equality Comparer.
        /// </param>
        /// <returns>A SemaphoreWithKeyDecoratorBuilder</returns>
        public SemaphoreWithKeyDecoratorBuilder<TMessageType, TKeyType> EqualityComparer<TKeyType>(IEqualityComparer<TKeyType> equalityComparer)
        {
            var newBuilder = new SemaphoreWithKeyDecoratorBuilder<TMessageType, TKeyType>().MaxNumberOfConcurrentMessages(this.maxNumberOfConcurrentMessages)
                .EqualityComparer(equalityComparer);
            this.buildFunc = newBuilder.Build;
            return newBuilder;
        }

        /// <summary>
        /// Sets the key selector
        /// </summary>
        /// <typeparam name="TKeyType">
        /// The key type
        /// </typeparam>
        /// <param name="keySelector">
        /// The key selector
        /// </param>
        /// <returns>
        /// A builder
        /// </returns>
        public SemaphoreWithKeyDecoratorBuilder<TMessageType, TKeyType> KeySelector<TKeyType>(Func<TMessageType, TKeyType> keySelector)
        {
            var newBuilder = new SemaphoreWithKeyDecoratorBuilder<TMessageType, TKeyType>().MaxNumberOfConcurrentMessages(this.maxNumberOfConcurrentMessages)
                .KeySelector(keySelector);
            this.buildFunc = newBuilder.Build;
            return newBuilder;
        }

        /// <summary>
        ///     Sets the key semaphore
        /// </summary>
        /// <typeparam name="TKeyType">The key types</typeparam>
        /// <param name="keySemaphore">A key semaphore</param>
        /// <returns>A SemaphoreWithKeyDecoratorBuilder</returns>
        public SemaphoreWithKeyDecoratorBuilder<TMessageType, TKeyType> KeySemaphore<TKeyType>(KeySemaphore<TKeyType> keySemaphore)
        {
            var newBuilder = new SemaphoreWithKeyDecoratorBuilder<TMessageType, TKeyType>().MaxNumberOfConcurrentMessages(this.maxNumberOfConcurrentMessages)
                .KeySemaphore(keySemaphore);
            this.buildFunc = newBuilder.Build;
            return newBuilder;
        }

        /// <summary>
        ///     Limits the message handler chain to X concurrent messages being handled.
        ///     This does not add concurrency but limits it.
        /// </summary>
        /// <param name="maxNumberOfConcurrentMessages">The number of concurrent messages</param>
        /// <returns>A builder</returns>
        // ReSharper disable once ParameterHidesMember
        public SemaphoreDecoratorBuilder<TMessageType> MaxNumberOfConcurrentMessages(int maxNumberOfConcurrentMessages)
        {
            this.maxNumberOfConcurrentMessages = maxNumberOfConcurrentMessages;
            return this;
        }

        internal ChainDecorator<TMessageType> Build(Func<TMessageType, CancellationToken, Task> nextHandler)
        {
            return this.buildFunc(nextHandler);
        }

        private ChainDecorator<TMessageType> BuildInternal(Func<TMessageType, CancellationToken, Task> nextHandler)
        {
            return new SemaphoreDecorator<TMessageType>(nextHandler, this.maxNumberOfConcurrentMessages);
        }
    }
}