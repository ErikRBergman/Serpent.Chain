// ReSharper disable StyleCop.SA1126 - R#/StyleCop is wrong
namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    /// The Retry Wire Up
    /// </summary>
    public class RetryWireUp : BaseWireUp<RetryAttribute>
    {
        /// <summary>
        /// Wire up a retry
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandlerType">The message handler type</typeparam>
        /// <param name="retryAttribute">The retry attribute</param>
        /// <param name="messageHandlerChainBuilder">The MCH builder</param>
        /// <param name="handler">The message handler</param>
        public override void WireUp<TMessageType, THandlerType>(RetryAttribute retryAttribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
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
    }
}