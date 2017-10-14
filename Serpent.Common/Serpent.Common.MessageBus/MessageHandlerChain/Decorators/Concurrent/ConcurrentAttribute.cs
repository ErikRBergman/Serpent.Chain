// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    /// When a handler decorated witht his attribute is Wired up, it's parallelized
    /// </summary>
    public class ConcurrentAttribute : WireUpAttribute
    {
        /// <summary>
        /// Make the handler run up to X concurrent messages
        /// </summary>
        /// <param name="maxNumberOfConcurrentMessages">The maximum number of concurrent messages</param>
        public ConcurrentAttribute(int maxNumberOfConcurrentMessages)
        {
            this.MaxNumberOfConcurrentMessages = maxNumberOfConcurrentMessages;
        }

        /// <summary>
        /// The maximum number of concurrent messages
        /// </summary>
        public int MaxNumberOfConcurrentMessages { get; }
    }
}