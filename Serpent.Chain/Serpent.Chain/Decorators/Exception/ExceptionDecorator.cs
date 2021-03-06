﻿namespace Serpent.Chain.Decorators.Exception
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class ExceptionDecorator<TMessageType> : ChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Exception, CancellationToken, Task<bool>> exceptionHandlerFunc;

        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        public ExceptionDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, Exception, CancellationToken, Task<bool>> exceptionHandlerFunc)
        {
            this.handlerFunc = handlerFunc;
            this.exceptionHandlerFunc = exceptionHandlerFunc;
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            try
            {
                await this.handlerFunc(message, token).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (await this.exceptionHandlerFunc(message, exception, token).ConfigureAwait(false))
                {
                    throw;
                }
            }
        }
    }
}