namespace Serpent.Common.MessageBus
{
    using System;

    internal class Readme
    {
        private void RetrySubscriptionExample()
        {
            var bus = new ConcurrentMessageBus<int>();

            bus.CreateRetrySubscription(
                message =>
                    {
                        throw new Exception("Handler failed");
                    }, 
                10,
                TimeSpan.FromMilliseconds(500));
        }

        private void FireAndForgetNew()
        {
            // Instantiate a new FireAndForgetPublisher
            var bus = new ConcurrentMessageBus<int>(options => { options.BusPublisher = new FireAndForgetPublisher<int>(); });
        }

        private void FireAndForgetDefault()
        {
            // Using the default FireAndForgetPublisher
            var bus = new ConcurrentMessageBus<int>(options =>
            {
                options.BusPublisher = FireAndForgetPublisher<int>.Default;
            });
        }

        private void FireAndForgetExtension()
        {
            // Using the options extension method
            var bus = new ConcurrentMessageBus<int>(
                options =>
                    {
                        options.UseFireAndForgetPublisher();
                    });
        }

        private void FireAndForgetExtensionDecorate()
        {
            // Using the options extension method to decorate ParallelPublisher with FireAndForgetPublisher
            var bus = new ConcurrentMessageBus<int>(
                options =>
                    {
                        options.UseFireAndForgetPublisher(ParallelPublisher<int>.Default);
                    });
        }

    }
}
