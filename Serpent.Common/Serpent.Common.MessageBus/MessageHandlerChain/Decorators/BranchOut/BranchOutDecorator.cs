namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.BranchOut
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The branch out decorator - used bu the branch out extensions
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    internal class BranchOutDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>, IMessageBusSubscriptions<TMessageType>
    {
        private readonly List<Func<TMessageType, CancellationToken, Task>> handlers;

        public BranchOutDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches)
        {
            var numberOfHandlers = (branches?.Length ?? 0) + 1;
            this.handlers = new List<Func<TMessageType, CancellationToken, Task>>(numberOfHandlers)
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

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            return Task.WhenAll(this.handlers.Select(h => h(message, token)));
        }

        public IMessageBusSubscription Subscribe(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            this.handlers.Add(handlerFunc);
            return null;
        }
    }
}