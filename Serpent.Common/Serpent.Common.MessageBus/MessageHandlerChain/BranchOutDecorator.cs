namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class BranchOutDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>, IMessageBusSubscriptions<TMessageType>
    {
        private readonly List<Func<TMessageType, Task>> handlers;

        public BranchOutDecorator(Func<TMessageType, Task> handlerFunc, params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches)
        {
            var numberOfHandlers = (branches?.Length ?? 0) + 1;
            this.handlers = new List<Func<TMessageType, Task>>(numberOfHandlers)
                {
                    handlerFunc
                };

            if (branches == null || branches.Length == 0)
            {
                return;
            }

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