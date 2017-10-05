namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Branch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class BranchHandler<TMessageType> : MessageHandlerChainDecorator<TMessageType>, IMessageBusSubscriptions<TMessageType>
    {
        private readonly List<Func<TMessageType, CancellationToken, Task>> handlers;

        public BranchHandler(params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches)
        {
            var numberOfHandlers = branches.Length;
            this.handlers = new List<Func<TMessageType, CancellationToken, Task>>(numberOfHandlers);

            foreach (var branch in branches)
            {
                var builder = new MessageHandlerChainBuilder<TMessageType>(this);
                branch(builder);
            }
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            return Task.WhenAll(this.handlers.Select(h => h(message, token)));
        }

        public IMessageBusSubscription Subscribe(Func<TMessageType, CancellationToken, Task> invocationFunc)
        {
            this.handlers.Add(invocationFunc);
            return null;
        }
    }
}