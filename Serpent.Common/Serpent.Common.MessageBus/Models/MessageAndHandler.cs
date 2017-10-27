namespace Serpent.Common.MessageBus.Models
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The message and handler type
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public struct MessageAndHandler<TMessageType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageAndHandler{TMessageType}"/> struct. 
        /// </summary>
        /// <param name="message">
        /// The message
        /// </param>
        /// <param name="messageHandler">
        /// The message handler
        /// </param>
        public MessageAndHandler(TMessageType message, Func<TMessageType, CancellationToken, Task> messageHandler)
        {
            this.Message = message;
            this.MessageHandler = messageHandler;
        }

        /// <summary>
        /// The message
        /// </summary>
        public TMessageType Message { get; }

        /// <summary>
        /// The message handler
        /// </summary>
        public Func<TMessageType, CancellationToken, Task> MessageHandler { get; }
    }
}