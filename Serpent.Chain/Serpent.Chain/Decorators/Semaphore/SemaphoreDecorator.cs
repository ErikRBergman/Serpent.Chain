﻿namespace Serpent.Chain.Decorators.Semaphore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SemaphoreDecorator<TMessageType> : ChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly SemaphoreSlim semaphore;

        public SemaphoreDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, int maxNumberOfConcurrentMessages)
        {
            this.handlerFunc = handlerFunc;
            this.MaxNumberOfConcurrentMessages = maxNumberOfConcurrentMessages;
            this.semaphore = new SemaphoreSlim(maxNumberOfConcurrentMessages);
        }

        private int MaxNumberOfConcurrentMessages { get; }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            await this.semaphore.WaitAsync(token).ConfigureAwait(false);

            try
            {
                await this.handlerFunc(message, token).ConfigureAwait(false);
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}