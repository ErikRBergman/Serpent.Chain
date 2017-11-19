namespace Serpent.MessageBus.MessageHandlerChain.Decorators.NoDuplicates
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.MessageHandlerChain.Decorators.Distinct;

    /// <summary>
    ///     Provides a builder for the distinct decorator
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public class NoDuplicatesDecoratorBuilder<TMessageType> : DecoratorBuilder<TMessageType>
    {
        private Func<Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>>> buildMethod;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NoDuplicatesDecoratorBuilder{TMessageType}" /> class.
        /// </summary>
        public NoDuplicatesDecoratorBuilder()
        {
            this.buildMethod = InternalBuildDecorator;
        }

        /// <summary>
        ///     Builds a new decorator
        /// </summary>
        /// <returns>The decorator</returns>
        public override Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> BuildDecorator()
        {
            return this.buildMethod();
        }

        /// <summary>
        ///     Sets the key equality comparer
        /// </summary>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="equalityComparer">The key equality comparer</param>
        /// <returns>A builder</returns>
        public NoDuplicatesDecoratorBuilder<TMessageType, TKeyType> EqualityComparer<TKeyType>(IEqualityComparer<TKeyType> equalityComparer)
        {
            equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
            var builder = this.CreateKeyBuilder<TKeyType>().EqualityComparer(equalityComparer);
            return builder;
        }

        /// <summary>
        ///     Sets the key equality comparer
        /// </summary>
        /// <param name="equalityComparer">The key equality comparer</param>
        /// <returns>A builder</returns>
        public NoDuplicatesDecoratorBuilder<TMessageType, TMessageType> EqualityComparer(IEqualityComparer<TMessageType> equalityComparer)
        {
            equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
            var builder = this.CreateKeyBuilder<TMessageType>().EqualityComparer(equalityComparer);
            builder.KeySelector(m => m);

            return builder;
        }

        /// <summary>
        ///     Sets the key selector
        /// </summary>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="keySelector">The key selector</param>
        /// <returns>A builder</returns>
        public NoDuplicatesDecoratorBuilder<TMessageType, TKeyType> KeySelector<TKeyType>(Func<TMessageType, TKeyType> keySelector)
        {
            keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            return this.CreateKeyBuilder<TKeyType>().KeySelector(keySelector);
        }

        private static Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> InternalBuildDecorator()
        {
            return innerHandler => new DistinctDecorator<TMessageType, TMessageType>(innerHandler, m => m).HandleMessageAsync;
        }

        private NoDuplicatesDecoratorBuilder<TMessageType, TKeyType> CreateKeyBuilder<TKeyType>()
        {
            var builder = new NoDuplicatesDecoratorBuilder<TMessageType, TKeyType>();
            this.buildMethod = builder.BuildDecorator;
            return builder;
        }
    }
}