namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Threading.Tasks;

    public class FilterSubscription<TMessageType> : BusSubscription<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly Func<TMessageType, Task<bool>> beforeInvoke;

        private readonly Func<TMessageType, Task> afterInvoke;

        public FilterSubscription(Func<TMessageType, Task> handlerFunc, Func<TMessageType, Task<bool>> beforeInvoke = null, Func<TMessageType, Task> afterInvoke = null)
        {
            this.handlerFunc = handlerFunc;
            this.beforeInvoke = beforeInvoke;
            this.afterInvoke = afterInvoke;
        }

        public FilterSubscription(BusSubscription<TMessageType> innerSubscription, Func<TMessageType, Task<bool>> beforeInvoke = null, Func<TMessageType, Task> afterInvoke = null)
        {
            this.beforeInvoke = beforeInvoke;
            this.afterInvoke = afterInvoke;
            this.handlerFunc = innerSubscription.HandleMessageAsync;
        }

        public override async Task HandleMessageAsync(TMessageType message)
        {
            bool invoke = false;

            if (this.beforeInvoke != null)
            {
                invoke = await this.beforeInvoke(message);
            }

            if (invoke)
            {
                await this.handlerFunc(message);
            }

            if (this.afterInvoke != null)
            {
                await this.afterInvoke(message);
            }
        }
    }
}