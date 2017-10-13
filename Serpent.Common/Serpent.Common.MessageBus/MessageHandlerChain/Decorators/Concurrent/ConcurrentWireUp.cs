﻿namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Concurrent
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public class ConcurrentWireUp : BaseWireUp<ConcurrentAttribute, ConcurrentConfiguration>
    {
        protected override ConcurrentConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            if (int.TryParse(text, out var concurrencyLevel))
            {
                return new ConcurrentConfiguration()
                           {
                               MaxNumberOfConcurrentMessages = concurrencyLevel
                           };
            }

            throw new Exception("Concurrent: Could not convert concurrency level to integer: " + text);
        }

        protected override void WireUpFromAttribute<TMessageType, THandlerType>(ConcurrentAttribute attribute, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            messageHandlerChainBuilder.Concurrent(attribute.MaxNumberOfConcurrentMessages);
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(ConcurrentConfiguration configuration, IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, THandlerType handler)
        {
            messageHandlerChainBuilder.Concurrent(configuration.MaxNumberOfConcurrentMessages);
        }
    }
}