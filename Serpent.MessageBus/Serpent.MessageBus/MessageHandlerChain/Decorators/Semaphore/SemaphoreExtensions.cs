// ReSharper disable once CheckNamespace
namespace Serpent.MessageBus
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
        /// <param name="messageHandlerChainBuilder">
        ///     The message handler chain builder
        /// </param>
        /// <param name="config">
        ///     The action called to configure the semaphore
        /// </param>
        /// <returns>
        ///     The <see cref="IMessageHandlerChainBuilder&lt;TMessageType&gt;" />.
        /// </returns>
        public static IMessageHandlerChainBuilder<TMessageType> Semaphore<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<SemaphoreDecoratorBuilder<TMessageType>> config)
        {
            var builder = new SemaphoreDecoratorBuilder<TMessageType>();
            config(builder);
            return messageHandlerChainBuilder.AddDecorator(currentHandler => builder.Build(currentHandler));
        }
    }
}