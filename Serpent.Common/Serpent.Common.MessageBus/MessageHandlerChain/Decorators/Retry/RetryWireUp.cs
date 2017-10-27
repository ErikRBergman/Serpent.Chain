// ReSharper disable StyleCop.SA1126 - R#/StyleCop is wrong
namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    /// The Retry Wire Up
    /// </summary>
    internal class RetryWireUp : BaseWireUp<RetryAttribute, RetryConfiguration>
    {
        protected override RetryConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new Exception("Retry: Could not convert the value to \"{numberOfAttempts};{retryDelay}\": " + text);
            }

            var split = text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
            {
                throw new Exception("Retry: Could not convert the value to \"{numberOfAttempts};{retryDelay}\": " + text);
            }

            if (int.TryParse(split[0], out var attempts) == false)
            {
                throw new Exception("Retry: Could not convert the value to \"{numberOfAttempts};{retryDelay}\": " + text);
            }

            if (TimeSpan.TryParse(split[1], out var delay) == false)
            {
                throw new Exception("Retry: Could not convert the value to \"{numberOfAttempts};{retryDelay}\": " + text);
            }

            return new RetryConfiguration
            {
                MaxNumberOfRetries = attempts,
                RetryDelay = delay,
                UseIMessageHandlerRetry = true
            };
        }

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
                messageHandlerChainBuilder.Retry(retryAttribute.MaxNumberOfAttempts, retryAttribute.RetryDelay, retryHandler);
            }
            else
            {
                messageHandlerChainBuilder.Retry(retryAttribute.MaxNumberOfAttempts, retryAttribute.RetryDelay);
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