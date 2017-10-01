namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class FirstDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, bool> predicate;

        private int wasReceived;

        public FirstDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, bool> predicate)
        {
            this.handlerFunc = handlerFunc;
            this.predicate = predicate;
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.wasReceived == 0)
            {
                if (this.predicate(message))
                {
                    if (Interlocked.CompareExchange(ref this.wasReceived, 1, 0) == 0)
                    {
                        return this.handlerFunc(message, token);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}