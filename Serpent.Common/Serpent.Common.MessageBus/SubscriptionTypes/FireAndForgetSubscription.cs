namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Threading.Tasks;

    public class FireAndForgetSubscription<T> : BusSubscription<T>
    {
        private readonly Func<T, Task> handlerFunc;

        public FireAndForgetSubscription(Func<T, Task> handlerFunc)
        {
            this.handlerFunc = handlerFunc;
        }

        public override Task HandleMessageAsync(T message)
        {
            Task.Run(() => this.handlerFunc(message));
            return Task.CompletedTask;
        }
    }
}