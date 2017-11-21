// ReSharper disable ParameterHidesMember

namespace Serpent.MessageHandlerChain.Decorators.Action
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class ActionDecoratorBuilder<TMessageType> : IActionDecoratorBuilder<TMessageType>, IDecoratorBuilder<TMessageType>
    {
        private Func<TMessageType, CancellationToken, Task> beforeFunc;

        private Func<TMessageType, CancellationToken, Exception, Task> finallyFunc;

        private Func<TMessageType, Task> onCancelFunc;

        private Func<TMessageType, Exception, Task<bool>> onExceptionFunc;

        private Func<TMessageType, Task> onSuccessFunc;

        private bool AreAllFuncsNull =>
            this.beforeFunc == null && this.onSuccessFunc == null && this.onCancelFunc == null && this.onExceptionFunc == null && this.finallyFunc == null;

        public IActionDecoratorBuilder<TMessageType> Before(Func<TMessageType, CancellationToken, Task> beforeFunc)
        {
            this.beforeFunc = beforeFunc;
            return this;
        }

        public Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> BuildDecorator()
        {
            if (this.AreAllFuncsNull)
            {
                return innerMessageHandler => innerMessageHandler;
            }

            return innerMessageHandler =>
                {
                    return async (message, token) =>
                        {
                            Exception lastException = null;

                            try
                            {
                                if (this.beforeFunc != null)
                                {
                                    await this.beforeFunc(message, token).ConfigureAwait(false);
                                }

                                if (token.IsCancellationRequested)
                                {
                                    await this.onCancelFunc(message).ConfigureAwait(false);
                                    return;
                                }

                                try
                                {
#pragma warning disable CC0031 // Check for null before calling a delegate
                                    await innerMessageHandler(message, token).ConfigureAwait(false);
#pragma warning restore CC0031 // Check for null before calling a delegate

                                    if (this.onSuccessFunc != null)
                                    {
                                        await this.onSuccessFunc(message).ConfigureAwait(false);
                                    }
                                }
                                catch (OperationCanceledException exception)
                                {
                                    lastException = exception;
                                    await this.onCancelFunc(message).ConfigureAwait(false);
                                    throw;
                                }
                                catch (Exception exception)
                                {
                                    lastException = exception;
                                    if (this.onExceptionFunc != null)
                                    {
                                        if (await this.onExceptionFunc(message, exception).ConfigureAwait(false))
                                        {
                                            throw;
                                        }
                                    }
                                    else
                                    {
                                        throw;
                                    }
                                }
                            }
                            finally
                            {
                                if (this.finallyFunc != null)
                                {
                                    await this.finallyFunc(message, token, lastException).ConfigureAwait(false);
                                }
                            }
                        };
                };
        }

        public IActionDecoratorBuilder<TMessageType> Finally(Func<TMessageType, CancellationToken, Exception, Task> finallyFunc)
        {
            this.finallyFunc = finallyFunc;
            return this;
        }

        public IActionDecoratorBuilder<TMessageType> OnCancel(Func<TMessageType, Task> onCancelFunc)
        {
            this.onCancelFunc = onCancelFunc;
            return this;
        }

        public IActionDecoratorBuilder<TMessageType> OnException(Func<TMessageType, Exception, Task<bool>> onExceptionFunc)
        {
            this.onExceptionFunc = onExceptionFunc;
            return this;
        }

        public IActionDecoratorBuilder<TMessageType> OnSuccess(Func<TMessageType, Task> onSuccessFunc)
        {
            this.onSuccessFunc = onSuccessFunc;
            return this;
        }
    }
}