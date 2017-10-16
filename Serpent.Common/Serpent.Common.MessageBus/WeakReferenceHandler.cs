namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    internal struct WeakReferenceHandler<TMessageType> : IMessageHandler<TMessageType>
    {
        private readonly WeakReference<Func<TMessageType, CancellationToken, Task>> subscriptionHandlerFunc;

        public WeakReferenceHandler(Func<TMessageType, CancellationToken, Task> subscriptionHandlerFunc)
        {
            this.subscriptionHandlerFunc = new WeakReference<Func<TMessageType, CancellationToken, Task>>(subscriptionHandlerFunc);
        }

        public Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken)
        {
            if (this.subscriptionHandlerFunc.TryGetTarget(out var target))
            {
                return target(message, cancellationToken);
            }

            return Task.CompletedTask;
        }
    }
}