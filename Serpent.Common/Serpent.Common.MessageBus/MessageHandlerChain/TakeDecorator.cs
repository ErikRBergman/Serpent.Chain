﻿namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TakeDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private int count;

        public TakeDecorator(Func<TMessageType, Task> handlerFunc, int numberOfMessages)
        {
            this.handlerFunc = handlerFunc;
            this.count = numberOfMessages;
        }

        public override Task HandleMessageAsync(TMessageType message)
        {
            if (this.count > 0)
            {
                var ourCount = Interlocked.Decrement(ref this.count);

                if (ourCount >= 0)
                {
                    return this.handlerFunc(message);
                }
            }

            return Task.CompletedTask;
        }
    }
}