namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public class BranchSubscription<TMessageType> : BusSubscription<TMessageType>, IMessageBusSubscriber<TMessageType>
    {
        private readonly List<Func<TMessageType, Task>> handlers;

        public BranchSubscription(Func<TMessageType, Task> handlerFunc, params Action<SubscriptionBuilder<TMessageType>>[] branches)
        {
            int numberOfHandlers = (branches?.Length ?? 0) + 1;
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
                var builder = new SubscriptionBuilder<TMessageType>(this);
                branch(builder);
            }
        }

        public override async Task HandleMessageAsync(TMessageType message)
        {
            foreach (var branch in this.handlers)
            {
                await branch(message);
            }
        }

        public IMessageBusSubscription Subscribe(Func<TMessageType, Task> invocationFunc)
        {
            this.handlers.Add(invocationFunc);
            return null;
        }
    }
}