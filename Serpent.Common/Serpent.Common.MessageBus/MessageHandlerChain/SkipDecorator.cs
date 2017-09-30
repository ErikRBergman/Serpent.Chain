namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class SkipDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private int count;

        public SkipDecorator(Func<TMessageType, Task> handlerFunc, int numberOfMessages)
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
                    return Task.CompletedTask;
                }
            }

            return this.handlerFunc(message);
        }
    }
}