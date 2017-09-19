namespace Serpent.Common.MessageBus
{
    internal class Readme
    {
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
