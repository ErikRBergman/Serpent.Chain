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

    internal class ReadmeHandlerSignatures
    {
        public ReadmeHandlerSignatures(IMessageBusSubscriber<ExampleMessage> bus)
        {
            // Normal method handler
            var subscription = bus
                .Subscribe()
                .Handler(this.HandleTheMessageAsync);


        }

        private async Task HandleTheMessageAsync(ExampleMessage message)
        {
        }

        private static async Task HandleTheMessageStaticAsync(ExampleMessage message)
        {
        }



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
}

