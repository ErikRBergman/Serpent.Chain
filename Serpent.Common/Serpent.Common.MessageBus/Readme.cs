#if DEBUG
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    internal class ExampleMessage
    {
        public string Id { get; set; }
    }

    internal class ExampleMessage2
    {
        public string Sid { get; set; }
    }

    internal class ReadmeBranch
    {

        /// <summary>
        /// Branch()
        /// </summary>
        /// <param name="bus"></param>
        public ReadmeBranch(IMessageBusSubscriber<ExampleMessage> bus)
        {
            bus
                .Subscribe()
                .NoDuplicates(message => message.Id)
                .Branch(
                    branch => branch
                        .Delay(TimeSpan.FromSeconds(10))
                        .Filter(message => message.Id == "Message 2")
                        .Handler(message => Console.WriteLine("I only handle Message 2")))
                .Handler(message => Console.WriteLine("I handle all messages"));

        }


    }

    internal class ReadmeFactoryHandler : IMessageHandler<ExampleMessage>, IDisposable
    {
        public void Dispose()
        {
        }

        public async Task HandleMessageAsync(ExampleMessage message)
        {
        }
    }


    internal class ReadmeExceptionAndFilter
    {
        public ReadmeExceptionAndFilter(IMessageBusSubscriber<ExampleMessage> bus)
        {
            var subscription = bus
                .Subscribe()
                .Exception(
                (message, exception) =>
                {
                    Console.WriteLine("Error! " + exception);
                    return true; // Propagate the exception up the chain
                })
                .Filter(
                    message => Console.WriteLine("Before the message handler is invoked"),
                    message => Console.WriteLine("The message handler succeeded as far as we know"))
                        .Handler(async message =>
                            {
                                await this.SomeMethodAsync();
                                Console.WriteLine(message.Id);
                            });
        }

        private Task SomeMethodAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class ReadmeFactoryHandlerSetup
    {
        public void SetupSubscription(IMessageBusSubscriber<ExampleMessage> bus)
        {
            bus
                .Subscribe()
                .Factory(() => new ReadmeFactoryHandler());
        }
    }

    internal class ReadmeHandler : IMessageHandler<ExampleMessage>, IMessageHandler<ExampleMessage2>
    {
        public async Task HandleMessageAsync(ExampleMessage message)
        {
        }

        public async Task HandleMessageAsync(ExampleMessage2 message)
        {
        }
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

#endif