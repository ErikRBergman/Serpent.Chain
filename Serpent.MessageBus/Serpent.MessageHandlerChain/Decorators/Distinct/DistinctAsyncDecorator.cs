namespace Serpent.MessageHandlerChain.Decorators.Distinct
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal class DistinctAsyncDecorator<TMessageType, TKeyType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly ConcurrentDictionary<TKeyType, bool> keyDictionary = new ConcurrentDictionary<TKeyType, bool>();

        private readonly Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector;

        private int isDefaultInvoked;

        public DistinctAsyncDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector, IEqualityComparer<TKeyType> equalityComparer = null)
        {
            this.handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
            this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

            if (equalityComparer != null)
            {
                this.keyDictionary = new ConcurrentDictionary<TKeyType, bool>(equalityComparer);
            }
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            var key = await this.keySelector(message, token).ConfigureAwait(false);

            if (key == null)
            {
                if (Interlocked.CompareExchange(ref this.isDefaultInvoked, 1, 0) == 0)
                {
                    await this.handlerFunc(message, token).ConfigureAwait(false);
                }

                return;
            }

            if (this.keyDictionary.TryAdd(key, true))
            {
                await this.handlerFunc(message, token).ConfigureAwait(false);
            }
        }
    }
}