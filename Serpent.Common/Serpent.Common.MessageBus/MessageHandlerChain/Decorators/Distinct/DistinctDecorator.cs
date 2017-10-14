namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Distinct
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    internal class DistinctDecorator<TMessageType, TKeyType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly ConcurrentDictionary<TKeyType, bool> keyDictionary = new ConcurrentDictionary<TKeyType, bool>();

        private readonly Func<TMessageType, TKeyType> keySelector;

        private int isDefaultInvoked;

        public DistinctDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, TKeyType> keySelector)
        {
            this.handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
            this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public DistinctDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, TKeyType> keySelector, IEqualityComparer<TKeyType> equalityComparer)
        {
            this.handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
            this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            this.keyDictionary = new ConcurrentDictionary<TKeyType, bool>(equalityComparer);
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            var key = this.keySelector(message);

            if (key == null)
            {
                if (Interlocked.CompareExchange(ref this.isDefaultInvoked, 1, 0) == 0)
                {
                    return this.handlerFunc(message, token);
                }

                return Task.CompletedTask;
            }

            if (this.keyDictionary.TryAdd(key, true))
            {
                return this.handlerFunc(message, token);
            }

            return Task.CompletedTask;
        }
    }
}