namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    public class FilteringMessageBusDectorator<T> : IMessageBus<T>
    {
        private readonly Func<T, Task<bool>> filterFunc;

        private readonly IMessageBus<T> innerMessageBus;

        public FilteringMessageBusDectorator(IMessageBus<T> innerMessageBus, Func<T, Task<bool>> filterFunc)
        {
            this.innerMessageBus = innerMessageBus;
            this.filterFunc = filterFunc;
        }

        public Task PublishEventAsync(T eventData)
        {
            return this.innerMessageBus.PublishEventAsync(eventData);
        }

        public IMessageBusSubscription Subscribe(Func<T, Task> invocationFunc)
        {
            return this.innerMessageBus.Subscribe(
                async message =>
                    {
                        if (await this.filterFunc(message))
                        {
                            await invocationFunc(message);
                        }
                    });
        }
    }
}