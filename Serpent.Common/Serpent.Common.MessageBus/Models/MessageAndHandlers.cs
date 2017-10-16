namespace Serpent.Common.MessageBus.Models
{
    using System.Collections.Generic;

    using Serpent.Common.MessageBus.Interfaces;

    /// <summary>
    /// The message and handlers type
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public struct MessageAndHandlers<TMessageType>
    {
        public MessageAndHandlers(TMessageType message, IEnumerable<IMessageHandler<TMessageType>> handlers)
        {
            this.Message = message;
            this.Handlers = handlers;
        }

        public TMessageType Message { get; }

        public IEnumerable<IMessageHandler<TMessageType>> Handlers { get; }
    }
}