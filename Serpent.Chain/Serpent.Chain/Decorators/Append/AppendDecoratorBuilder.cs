// ReSharper disable ParameterHidesMember

namespace Serpent.Chain.Decorators.Append
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Models;

    internal class AppendDecoratorBuilder<TMessageType> : IAppendDecoratorBuilder<TMessageType>, IDecoratorBuilder<TMessageType>
    {
        private Func<TMessageType, CancellationToken, Task<TMessageType>> asyncMessageSelector;

        private Func<TMessageType, CancellationToken, Task<bool>> asyncPredicate;

        private bool isRecursive;

        public Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> BuildDecorator()
        {
            if (this.asyncMessageSelector == null)
            {
                throw new Exception("No message selector was specified");
            }

            var predicate = this.asyncPredicate;

            if (this.isRecursive == false && predicate == null)
            {
                return innerMessageHandler =>
                    {
                        return async (message, token) =>
                            {
#pragma warning disable CC0031 // Check for null before calling a delegate
                                var chainedMessageTask = innerMessageHandler(message, token);
#pragma warning restore CC0031 // Check for null before calling a delegate
                                await Task.WhenAll(chainedMessageTask, this.InnerMessageHandlerAsync(innerMessageHandler, new MessageAndToken<TMessageType>(message, token))).ConfigureAwait(false);
                            };
                    };
            }

            return innerMessageHandler => (message, token) => Task.WhenAll(
#pragma warning disable CC0031 // Check for null before calling a delegate
                innerMessageHandler(message, token),
#pragma warning restore CC0031 // Check for null before calling a delegate
                AppendWhenAsync(
                    new AppendAsyncParameters<TMessageType>
                    {
                        InnerMessageHandler = innerMessageHandler,
                        Predicate = this.asyncPredicate,
                        MessageSelector = this.asyncMessageSelector,
                        Message = message,
                        CancellationToken = token,
                        IsRecursive = this.isRecursive
                    }));
        }

        public IAppendDecoratorBuilder<TMessageType> Recursive(bool isRecursive = true)
        {
            this.isRecursive = isRecursive;
            return this;
        }

        public IAppendDecoratorBuilder<TMessageType> Select(Func<TMessageType, CancellationToken, Task<TMessageType>> asyncMessageSelector)
        {
            this.asyncMessageSelector = asyncMessageSelector;
            return this;
        }

        public IAppendDecoratorBuilder<TMessageType> Where(Func<TMessageType, CancellationToken, Task<bool>> predicate)
        {
            this.asyncPredicate = predicate;
            return this;
        }

        private static async Task AppendWhenAsync(AppendAsyncParameters<TMessageType> parameters)
        {
            TMessageType newMessage;

            if (parameters.Predicate == null)
            {
                newMessage = await parameters.MessageSelector(parameters.Message, parameters.CancellationToken).ConfigureAwait(false);
                if (newMessage == null || newMessage.Equals(default(TMessageType)))
                {
                    return;
                }
            }
            else
            {
                if (await parameters.Predicate(parameters.Message, parameters.CancellationToken).ConfigureAwait(false))
                {
                    newMessage = await parameters.MessageSelector(parameters.Message, parameters.CancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return;
                }
            }

            if (parameters.IsRecursive)
            {
                await Task.WhenAll(parameters.InnerMessageHandler(newMessage, parameters.CancellationToken), AppendWhenAsync(parameters.CloneForMessage(newMessage)));
            }
            else
            {
                await parameters.InnerMessageHandler(newMessage, parameters.CancellationToken).ConfigureAwait(false);
            }
        }

        private async Task InnerMessageHandlerAsync(
            Func<TMessageType, CancellationToken, Task> messageHandler,
            MessageAndToken<TMessageType> messageAndToken)
        {
            var newMessage = await this.asyncMessageSelector(messageAndToken.Message, messageAndToken.Token).ConfigureAwait(false);
#pragma warning disable CC0031 // Check for null before calling a delegate
            await messageHandler(newMessage, messageAndToken.Token).ConfigureAwait(false);
#pragma warning restore CC0031 // Check for null before calling a delegate
        }
    }
}