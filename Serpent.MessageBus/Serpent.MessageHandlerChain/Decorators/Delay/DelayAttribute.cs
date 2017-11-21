// ReSharper disable once CheckNamespace
namespace Serpent.MessageHandlerChain
{
    using System;

    using Serpent.MessageHandlerChain.WireUp;

    /// <summary>
    /// Make the handler class await a fixed time before calling the handler
    /// </summary>
    public sealed class DelayAttribute : WireUpAttribute
    {
        /// <summary>
        /// Delay a TimeSpan (D:HH:mm:ss.ms)
        /// </summary>
        /// <param name="delayInText">The time to delay the message handler for each messsage (D:HH:mm:ss.ms)</param>
        public DelayAttribute(string delayInText)
        {
            this.Delay = TimeSpan.Parse(delayInText);
        }

        /// <summary>
        /// Delay in seconds
        /// </summary>
        /// <param name="delayInSeconds">Delay in seconds. Can be fractions of seconds</param>
        public DelayAttribute(double delayInSeconds)
        {
            this.Delay = TimeSpan.FromSeconds(delayInSeconds);
        }

        /// <summary>
        /// The delay to wait
        /// </summary>
        public TimeSpan Delay { get; }
    }
}