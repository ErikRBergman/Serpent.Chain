namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class BranchHandler<TMessageType> : MessageHandlerChainDecorator<TMessageType>, IMessageBusSubscriber<TMessageType>
    {
        private readonly List<Func<TMessageType, Task>> handlers;

        public BranchHandler(params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches)
        {
            var numberOfHandlers = branches.Length;
            this.handlers = new List<Func<TMessageType, Task>>(numberOfHandlers);

            foreach (var branch in branches)
            {
                var builder = new MessageHandlerChainBuilder<TMessageType>(this);
                branch(builder);
            }
        }

        public override async Task HandleMessageAsync(TMessageType message)
        {
            foreach (var branch in this.handlers)
            {
                await branch(message).ConfigureAwait(false);
            }
        }

        public IMessageBusSubscription Subscribe(Func<TMessageType, Task> invocationFunc)
        {
            this.handlers.Add(invocationFunc);
            return null;
        }
    }
}