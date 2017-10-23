namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.NoDuplicates
{
    using System;

    public struct NoDuplicatesMessageAndTriggers<TMessageType>
    {
        /// <summary>
        /// The message
        /// </summary>
        public TMessageType Message { get; set; }

        /// <summary>
        /// Enable queuing of messages with this messages key.
        /// For example, when storing data from a memory representation, this method is called just before making a snapshot of the data,
        /// to ensure changes coming after this method call are queued and handled.
        /// </summary>
        public Action<TMessageType> EnableQueueing { get; set; }

        /// <summary>
        /// The method to call to drop the queue and stop queueing for this message's key.
        /// Usage: May be relevant when queueing has been enabled, but the handler fails, since this message is virtually requeued when retrying
        /// </summary>
        public Action<TMessageType> DropQueue { get; set; }
    }
}