// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System;

    /// <summary>
    ///     The semaphore MHCBuilder extensions
    /// </summary>
    public static class SemapshoreExtensions
    {
        /// <summary>
        ///     Limits the message handler chain to X concurrent messages being handled.
        ///     This method does not add concurrency but limits it.
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The type of the messages type in the message handler chain
        /// </typeparam>
        /// <param name="chainBuilder">
        ///     The message handler chain builder
        /// </param>
        /// <param name="config">
        ///     The action called to configure the semaphore
        /// </param>
        /// <returns>
        ///     The <see cref="IChainBuilder&lt;TMessageType&gt;" />.
        /// </returns>
        public static IChainBuilder<TMessageType> Semaphore<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Action<SemaphoreDecoratorBuilder<TMessageType>> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var builder = new SemaphoreDecoratorBuilder<TMessageType>();
            config(builder);
            return chainBuilder.AddDecorator(currentHandler => builder.Build(currentHandler));
        }
    }
}