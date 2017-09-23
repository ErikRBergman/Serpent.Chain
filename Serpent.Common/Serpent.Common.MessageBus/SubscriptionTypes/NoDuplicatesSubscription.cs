namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public class NoDuplicatesSubscription<TMessageType, TKeyType> : BusSubscription<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly Func<TMessageType, TKeyType> keySelector;

        private readonly ConcurrentDictionary<TKeyType, bool> keyDictionary = new ConcurrentDictionary<TKeyType, bool>();

        private int isDefaultInvoked = 0;

        public NoDuplicatesSubscription(Func<TMessageType, Task> handlerFunc, Func<TMessageType, TKeyType> keySelector)
        {
            this.handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
            this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public NoDuplicatesSubscription(Func<TMessageType, Task> handlerFunc, Func<TMessageType, TKeyType> keySelector, IEqualityComparer<TKeyType> equalityComparer)
        {
            this.handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
            this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            this.keyDictionary = new ConcurrentDictionary<TKeyType, bool>(equalityComparer);
        }

        public override async Task HandleMessageAsync(TMessageType message)
        {
            var key = this.keySelector(message);

            if (key == null)
            {
                if (Interlocked.CompareExchange(ref this.isDefaultInvoked, 1, 0) == 0)
                {
                    try
                    {
                        await this.handlerFunc(message);
                    }
                    finally
                    {
                        this.isDefaultInvoked = 0;
                    }
                }

                return;
            }

            if (this.keyDictionary.TryAdd(key, true))
            {
                try
                {
                    await this.handlerFunc(message);
                }
                finally
                {
                    this.keyDictionary.TryRemove(key, out _);
                }
            }
        }
    }
}