namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class RetryWireUp : BaseWireUp<RetryAttribute>
    {
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