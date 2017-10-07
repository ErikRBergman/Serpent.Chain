// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp;

    public class RetryAttribute : WireUpAttribute
    {
        public RetryAttribute(int maxNumberOfRetries, double retryDelayInSeconds)
        {
            this.MaxNumberOfRetries = maxNumberOfRetries;
            this.RetryDelay = TimeSpan.FromSeconds(retryDelayInSeconds);
        }

        public RetryAttribute(int maxNumberOfRetries, string retryDelayInText)
        {
            this.MaxNumberOfRetries = maxNumberOfRetries;
            this.RetryDelay = TimeSpan.Parse(retryDelayInText);
        }

        public bool UseIMessageHandlerRetry { get; set; } = true;

        public int MaxNumberOfRetries { get; }

        public TimeSpan RetryDelay { get; }
    }
}