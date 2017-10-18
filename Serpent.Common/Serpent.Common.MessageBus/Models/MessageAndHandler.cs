namespace Serpent.Common.MessageBus.Models
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public struct MessageAndHandler<TMessageType>
    {
        public MessageAndHandler(TMessageType message, Func<TMessageType, CancellationToken, Task> handler)
        {
            this.Message = message;
            this.Handler = handler;
        }

        public TMessageType Message { get; }

        public Func<TMessageType, CancellationToken, Task> Handler { get; }
    }
}