namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    internal class ExampleMessage
    {
        public string Id { get; set; }
    }


    internal class ReadmeExampleBus
    {
        public async Task Create()
        {
            // Create a message bus
            var bus = new ConcurrentMessageBus<ExampleMessage>();

            // Add a synchronous subscriber
            var synchronousSubscriber = bus
                    .Subscribe()
                    .Handler(message => Console.WriteLine(message.Id));

            // Add an asynchronous subscriber
            var asynchronousSubscriber = bus
                .Subscribe()
                .Handler(async message =>
                    {
                        await this.SomeMethodAsync();
                        Console.WriteLine(message.Id);
                    });

            // Publish a message to the bus
            await bus.PublishAsync(
                new ExampleMessage
                    {
                        Id = "Message 1"
                    });
        }

        public Task SomeMethodAsync()
        {
            return Task.CompletedTask;
        }
    }


    internal class Readme
    {
        private void RetrySubscriptionExample()
        {
            var bus = new ConcurrentMessageBus<int>();

            bus.CreateRetrySubscription(message => { throw new Exception("Handler failed"); }, 10, TimeSpan.FromMilliseconds(500));
        }

        private void FireAndForgetNew()
        {
            // Instantiate a new FireAndForgetPublisher
            var bus = new ConcurrentMessageBus<int>(options => { options.BusPublisher = new FireAndForgetPublisher<int>(); });
        }

        private void FireAndForgetDefault()
        {
            // Using the default FireAndForgetPublisher
            var bus = new ConcurrentMessageBus<int>(options => { options.BusPublisher = FireAndForgetPublisher<int>.Default; });
        }

        private void FireAndForgetExtension()
        {
            // Using the options extension method
            var bus = new ConcurrentMessageBus<int>(options => { options.UseFireAndForgetPublisher(); });
        }

        private void FireAndForgetExtensionDecorate()
        {
            // Using the options extension method to decorate ParallelPublisher with FireAndForgetPublisher
            var bus = new ConcurrentMessageBus<int>(options => { options.UseFireAndForgetPublisher(ParallelPublisher<int>.Default); });
        }
    }
}