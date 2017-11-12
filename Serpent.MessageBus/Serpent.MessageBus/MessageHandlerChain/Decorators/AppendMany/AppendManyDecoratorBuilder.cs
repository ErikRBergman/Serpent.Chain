// ReSharper disable ParameterHidesMember

namespace Serpent.MessageBus.MessageHandlerChain.Decorators.AppendMany
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

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
                                var chainedMessageTask = innerMessageHandler(message, token);
                                return Task.WhenAll(chainedMessageTask, InnerMessageHandlerAsync(innerMessageHandler, (msg, _) => this.asyncMessageSelector(msg, token), message, token));
                            };
                    };
            }

            return innerMessageHandler => 
                (message, token) => Task.WhenAll(
                    innerMessageHandler(message, token),
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

            await Task.WhenAll(newMessagesTask, Task.WhenAll(newMessages.Select(newMessage => AppendManyWhenAsync(parameters.CloneForMessage(newMessage)))));
        }

        private static async Task InnerMessageHandlerAsync(
            Func<TMessageType, CancellationToken, Task> messageHandler,
            Func<TMessageType, CancellationToken, Task<IEnumerable<TMessageType>>> messageSelector,
            TMessageType originalMessage,
            CancellationToken token)
        {
            var newMessages = await messageSelector(originalMessage, token).ConfigureAwait(false);
            if (newMessages != null)
            {
                await Task.WhenAll(newMessages.Select(message => messageHandler(message, token))).ConfigureAwait(false);
            }
        }
    }
}