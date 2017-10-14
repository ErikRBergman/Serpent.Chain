namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Distinct
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    internal class DistinctAsyncDecorator<TMessageType, TKeyType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly ConcurrentDictionary<TKeyType, bool> keyDictionary = new ConcurrentDictionary<TKeyType, bool>();

        private readonly Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector;

        private int isDefaultInvoked;

        public DistinctAsyncDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector)
        {
            this.handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
            this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public DistinctAsyncDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector, IEqualityComparer<TKeyType> equalityComparer)
        {
            this.handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
            this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            this.keyDictionary = new ConcurrentDictionary<TKeyType, bool>(equalityComparer);
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            var key = await this.keySelector(message, token);

            if (key == null)
            {
                if (Interlocked.CompareExchange(ref this.isDefaultInvoked, 1, 0) == 0)
                {
                    await this.handlerFunc(message, token);
                }

                return;
            }

            if (this.keyDictionary.TryAdd(key, true))
            {
                await this.handlerFunc(message, token);
            }
        }
    }
}