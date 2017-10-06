// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp;

    public class DelayAttribute : WireUpAttribute
    {
        public DelayAttribute(string delayInText)
        {
            this.Delay = TimeSpan.Parse(delayInText);
        }

        public DelayAttribute(double delayInSeconds)
        {
            this.Delay = TimeSpan.FromSeconds(delayInSeconds);
        }

        public TimeSpan Delay { get; }
    }
}