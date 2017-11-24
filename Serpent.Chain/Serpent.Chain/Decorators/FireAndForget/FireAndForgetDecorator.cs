namespace Serpent.Chain.Decorators.FireAndForget
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class FireAndForgetDecorator<TMessageType> : ChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        public FireAndForgetDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            this.handlerFunc = handlerFunc;
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            Task.Run(() => this.handlerFunc(message, token), token);
            return Task.CompletedTask;
        }
    }
}