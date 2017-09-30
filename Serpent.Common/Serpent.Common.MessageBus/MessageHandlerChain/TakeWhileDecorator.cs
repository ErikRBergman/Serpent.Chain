namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TakeWhileDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly Func<TMessageType, bool> predicate;

        private int isActive = 1;

        public TakeWhileDecorator(Func<TMessageType, Task> handlerFunc, Func<TMessageType, bool> predicate)
        {
            this.handlerFunc = handlerFunc;
            this.predicate = predicate;
        }

        public override Task HandleMessageAsync(TMessageType message)
        {
            if (this.isActive == 1)
            {
                if (this.predicate(message))
                {
                    return this.handlerFunc(message);
                }

                this.isActive = 0;
            }

            return Task.CompletedTask;
        }
    }
}