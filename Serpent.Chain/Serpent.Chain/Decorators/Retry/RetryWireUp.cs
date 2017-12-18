// ReSharper disable StyleCop.SA1126 - R#/StyleCop is wrong
namespace Serpent.Chain.Decorators.Retry
{
    using System;
    using System.Collections.Generic;

    using Serpent.Chain.WireUp;

    /// <summary>
    /// The Retry Wire Up
    /// </summary>
    internal class RetryWireUp : BaseWireUp<RetryAttribute, RetryConfiguration>
    {
        protected override RetryConfiguration CreateAndParseConfigurationFromDefaultValue(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new Exception("Retry: Could not convert the value to \"numberOfAttempts;retryDelay{;retryDelay;retryDelay...}\": " + text);
            }

            var split = text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 2)
            {
                throw new Exception("Retry: Could not convert the value to \"numberOfAttempts;retryDelay{;retryDelay;retryDelay...}\": " + text);
            }

            if (int.TryParse(split[0], out var attempts) == false)
            {
                throw new Exception("Retry: Could not convert the parameter 1 (" + split[0] + ") to \"numberOfAttempts;retryDelay{;retryDelay;retryDelay...}\": " + text);
            }

            var delays = new List<TimeSpan>(split.Length - 1);

            for (int i = 1; i < split.Length; i++)
            {
                if (TimeSpan.TryParse(split[i], out var delay) == false)
                {
                    throw new Exception("Retry: Could not convert parameter number " + (i + 1) + " (" + split[i] + ") the value to \"numberOfAttempts;retryDelay{;retryDelay;retryDelay...}\": " + text);
                }

                delays.Add(delay);
            }

            return new RetryConfiguration
            {
                MaxNumberOfAttempts = attempts,
                RetryDelays = delays,
                UseIMessageHandlerRetry = true
            };
        }

        /// <summary>
        /// Wire up a retry
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandlerType">The message handler type</typeparam>
        /// <param name="retryAttribute">The retry attribute</param>
        /// <param name="chainBuilder">The MCH builder</param>
        /// <param name="handler">The message handler</param>
        protected override void WireUpFromAttribute<TMessageType, THandlerType>(RetryAttribute retryAttribute, IChainBuilder<TMessageType> chainBuilder, THandlerType handler)
        {
            if (retryAttribute.UseIMessageHandlerRetry && handler is IMessageHandlerRetry<TMessageType> retryHandler)
            {
                chainBuilder.Retry(
                    b => b
                        .MaximumNumberOfAttempts(retryAttribute.MaxNumberOfAttempts)
                        .RetryDelays(retryAttribute.RetryDelays)
                        .RetryHandler(retryHandler));
            }
            else
            {
                chainBuilder.Retry(r => r.MaximumNumberOfAttempts(retryAttribute.MaxNumberOfAttempts).RetryDelays(retryAttribute.RetryDelays));
            }
        }

        protected override void WireUpFromConfiguration<TMessageType, THandlerType>(RetryConfiguration configuration, IChainBuilder<TMessageType> chainBuilder, THandlerType handler)
        {
            if (configuration.UseIMessageHandlerRetry && handler is IMessageHandlerRetry<TMessageType> retryHandler)
            {
                chainBuilder.Retry(
                    b => b
                        .MaximumNumberOfAttempts(configuration.MaxNumberOfAttempts)
                        .RetryDelays(configuration.RetryDelays)
                        .RetryHandler(retryHandler));
            }
            else
            {
                chainBuilder.Retry(
                    b => b
                        .MaximumNumberOfAttempts(configuration.MaxNumberOfAttempts)
                        .RetryDelays(configuration.RetryDelays));
            }
        }
    }
}