namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp;

    public class RetryWireUp : IWireUp
    {
        public Type AttributeType { get; } = typeof(RetryAttribute);

        public void WireUp<TMessageType, THandlerType>(Attribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            if (attribute is RetryAttribute retryAttribute)
            {
                if (retryAttribute.UseWireUpRetryMethod && handler is IMessageHandlerRetry<TMessageType> retryHandler)
                {
                    messageHandlerChainBuilder.Retry(retryAttribute.MaxNumberOfRetries, retryAttribute.RetryDelay, (message, exception, attempt, maxNumberOfAttempts) => retryHandler.HandleRetryAsync(message, exception, retryAttribute.RetryDelay, attempt, maxNumberOfAttempts));
                }
                else
                {
                    messageHandlerChainBuilder.Retry(retryAttribute.MaxNumberOfRetries, retryAttribute.RetryDelay);
                }
            }
        }
    }
}