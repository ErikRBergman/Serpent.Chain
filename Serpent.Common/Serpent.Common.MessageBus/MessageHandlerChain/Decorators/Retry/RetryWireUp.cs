// ReSharper disable StyleCop.SA1126 - R#/StyleCop is wrong
namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    /// The Retry Wire Up
    /// </summary>
    public class RetryWireUp : BaseWireUp<RetryAttribute, RetryConfiguration>
    {
        /// <summary>
        /// Wire up a retry
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandlerType">The message handler type</typeparam>
        /// <param name="retryAttribute">The retry attribute</param>
        /// <param name="messageHandlerChainBuilder">The MCH builder</param>
        /// <param name="handler">The message handler</param>
        protected override void WireUpFromAttribute<TMessageType, THandlerType>(RetryAttribute retryAttribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            if (retryAttribute.UseIMessageHandlerRetry && handler is IMessageHandlerRetry<TMessageType> retryHandler)
            {
                messageHandlerChainBuilder.Retry(retryAttribute.MaxNumberOfRetries, retryAttribute.RetryDelay, retryHandler);
            }
            else
            {
                messageHandlerChainBuilder.Retry(retryAttribute.MaxNumberOfRetries, retryAttribute.RetryDelay);
            }
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(RetryConfiguration configuration, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            if (configuration.UseIMessageHandlerRetry && handler is IMessageHandlerRetry<TMessageType> retryHandler)
            {
                messageHandlerChainBuilder.Retry(configuration.MaxNumberOfRetries, configuration.RetryDelay, retryHandler);
            }
            else
            {
                messageHandlerChainBuilder.Retry(configuration.MaxNumberOfRetries, configuration.RetryDelay);
            }
        }
    }
}