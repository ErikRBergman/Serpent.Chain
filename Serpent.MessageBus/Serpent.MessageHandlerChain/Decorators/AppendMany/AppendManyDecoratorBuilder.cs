// ReSharper disable ParameterHidesMember

namespace Serpent.MessageHandlerChain.Decorators.AppendMany
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Models;

    internal class AppendManyDecoratorBuilder<TMessageType> : IAppendManyDecoratorBuilder<TMessageType>, IDecoratorBuilder<TMessageType>
    {
        private Func<TMessageType, CancellationToken, Task<IEnumerable<TMessageType>>> asyncMessageSelector;

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
                        return (message, token) =>
                            {
#pragma warning disable CC0031 // Check for null before calling a delegate
                                var chainedMessageTask = innerMessageHandler(message, token);
#pragma warning restore CC0031 // Check for null before calling a delegate
                                return Task.WhenAll(chainedMessageTask, this.InnerMessageHandlerAsync(innerMessageHandler, new MessageAndToken<TMessageType>(message, token)));
                            };
                    };
            }

            return innerMessageHandler => 
                (message, token) => Task.WhenAll(
#pragma warning disable CC0031 // Check for null before calling a delegate
                    innerMessageHandler(message, token),
#pragma warning restore CC0031 // Check for null before calling a delegate
                    AppendManyWhenAsync(
                        new AppendManyAsyncParameters<TMessageType>
                            {
                                InnerMessageHandler = innerMessageHandler,
                                Predicate = this.asyncPredicate,
                                MessageSelector = this.asyncMessageSelector,
                                Message = message,
                                CancellationToken = token,
                                IsRecursive = this.isRecursive
                            }));
        }

        public IAppendManyDecoratorBuilder<TMessageType> Recursive(bool isRecursive = true)
        {
            this.isRecursive = isRecursive;
            return this;
        }

        public IAppendManyDecoratorBuilder<TMessageType> Select(Func<TMessageType, CancellationToken, Task<IEnumerable<TMessageType>>> asyncMessageSelector)
        {
            this.asyncMessageSelector = asyncMessageSelector;
            return this;
        }

        public IAppendManyDecoratorBuilder<TMessageType> Where(Func<TMessageType, CancellationToken, Task<bool>> predicate)
        {
            this.asyncPredicate = predicate;
            return this;
        }

        private static async Task AppendManyWhenAsync(AppendManyAsyncParameters<TMessageType> parameters)
        {
            IEnumerable<TMessageType> newMessages = null;

            if (parameters.Predicate == null)
            {
                newMessages = await parameters.MessageSelector(parameters.Message, parameters.CancellationToken).ConfigureAwait(false);
            }
            else
            {
                if (await parameters.Predicate(parameters.Message, parameters.CancellationToken).ConfigureAwait(false))
                {
                    newMessages = await parameters.MessageSelector(parameters.Message, parameters.CancellationToken).ConfigureAwait(false);
                }
            }

            if (newMessages == null)
            {
                return;
            }

            var newMessagesTask = Task.WhenAll(newMessages.Select(message => parameters.InnerMessageHandler(message, parameters.CancellationToken)));

            if (!parameters.IsRecursive)
            {
                await newMessagesTask.ConfigureAwait(false);
                return;
            }

            await Task.WhenAll(newMessagesTask, Task.WhenAll(newMessages.Select(newMessage => AppendManyWhenAsync(parameters.CloneForMessage(newMessage))))).ConfigureAwait(false);
        }

        private async Task InnerMessageHandlerAsync(
            Func<TMessageType, CancellationToken, Task> messageHandler,
            MessageAndToken<TMessageType> messageAndToken)
        {
            var newMessages = await this.asyncMessageSelector(messageAndToken.Message, messageAndToken.Token).ConfigureAwait(false);
            if (newMessages != null)
            {
#pragma warning disable CC0031 // Check for null before calling a delegate
                await Task.WhenAll(newMessages.Select(message => messageHandler(message, messageAndToken.Token))).ConfigureAwait(false);
#pragma warning restore CC0031 // Check for null before calling a delegate
            }
        }
    }
}