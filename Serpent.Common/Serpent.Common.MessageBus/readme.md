# Serpent.Common.MessageBus
This is an asynchronous .NET Standard 2.0 message bus for usage in any project where a message bus is suitable.
All messages are dispatched through the .NET TPL (which is included in .NET Framework, .NET Standard and .NET Core).
Serpent.Common.MessageBus is .NET Standard 2.0, which means, you can use it on any runtime that supports .NET Standard 2.0, for example .NET Framework 4.6.1 and .NET Core 1.0. 

The message bus is implemented by `ConcurrentMessageBus<TMessageType>` and has 3 interfaces:
* `IMessageBus<TMessageType>` which in turn has two interfaces:
* `IMessageBusPublisher<TMessageType>` used to publish messages to the bus
* `IMessageBusSubscriber<TMessageType>` used to subscribe to messages

A message bus communicates using messages. A message can be a simple type like an `integer`, a `struct` or a `class`. 

Feel free to fork the project, make changes and send pull requests, report errors, suggestions, ideas, ask questions etc.

## Why?
Why would I use Serpent.Common.MessageBus or any message bus in my application instead using normal method calls?
Well, I can come up with a few reasons.

* Loose coupling - Message publisher and the subscribers know nothing about each other. As long as they know about the bus and what the messages do, both subscribers and publishers can be changed, added or replaced witout affecting each other.
* A standardized way to add cross cutting concerns (like retry, thread synchronization)
* Concurrency made easy - By adding only 1 line of code (`.Concurrent(16)`), you can parallelize your work on the .NET thread pool
* Reuse - Smaller components with a defined contract can more easily be reused
* Flexibility and out of the box functionality. When you have created your message handler, you can add quite some out-of-the-box functionality to it without modifying the message handler. Throttling, Exception handling, Retry, Duplicate message elimination, to name a few.
* Configurability - Adding fancy functionality like `Retry()`, with one line of code. See Message chain decorators.

## How to install
If you use Visual Studio, open the NuGet client for your project and find `Serpent.Common.MessageBus`.

or

Open the `Package Manager Console` and type:

`install-package Serpent.Common.MessageBus`

To start using the bus, add
```csharp
using Serpent.Common.MessageBus;
```

## Example
```csharp
using Serpent.Common.MessageBus;

internal class ExampleMessage
{
    public string Id { get; set; }
}

public class Program
{
    public static void Main()
    {
        // Create a message bus
        var bus = new ConcurrentMessageBus<ExampleMessage>();

        // Add a synchronous subscriber
        var subscription = bus
                .Subscribe()
                // .Retry(3, TimeSpan.FromSeconds(30)) // Try up to 3 times with 30 sec. delay in between
                // .Concurrent(16) // Up to 16 concurrent tasks will handle messges
                .Handler(message => Console.WriteLine(message.Id));

        // Add an asynchronous subscriber
        var asynchronousSubscription = bus
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
    }
```

## Subscribing to messages
When you subscribe to a bus, you need to specify a message handler. The handler is what does something when a message is published (sent to the bus).

This is a simple synchronous subscription:
```csharp
var subscription = bus
    .Subscribe()
    .Handler(message =>
        {
            Console.WriteLine(message.Id);
        });
```
Note! The message bus works internally fully with TPL and if you need asynchronous code (like I/O), use one of the overloads returning a `Task`. If you don't need any async operations, and you want to log stuff (to a logger that is already asynchronous), you can create a synchronous subscription.


A message handler can be one of the following:
* An inline method with one of the following signatures
  * `bus.Subscribe().Handler(message => { .. })`
  * `bus.Subscribe().Handler(message => { return Task.CompletedTask; })`
  * `bus.Subscribe().Handler((message, cancellationToken) => { return Task.CompletedTask; })`
* A method of your choice that has one of the following signatures (but you can pick the name yourself):
  * `void MyMethodOfChoice(MyMessageType message)`
  * `Task MyMethodOfChoiceAsync(MyMessageType message)`
  * `Task MyMethodOfChoiceasync(MyMessageType message, CancellationToken cancellationToken)`
* A type that implements `IMessageHandler<TMessageType>`
* A type that implements `ISimpleMessageHandler<TMessageType>`
* A factory instantiating (or returning) a type that implements `IMessageHandler<TMessageType>`.

A single handler can handle messages for multiple types, since every message is typed.

#### Subscribe and handle messages asynchronously
```csharp
var subscription = bus
    .Subscribe()
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });

// ...

// Unsubscribe
subscription.Dispose();
```
`.Handler()` returns an `IMessageBusSubscription` on which you call `.Dispose()` to unsubscribe.

#### Subscribe and handle with IMessageHandler

```csharp
internal class ExampleMessage
{
    public string Id { get; set; }
}

public class HandlerClass: IMessageHandler<ExampleMessage>
{
	public async Task HandleMessageAsync(ExampleMessage message)
	{
		Console.WriteLine(message.Id);
	}
}

var handler = new HandlerClass();

var subscription = bus
    .Subscribe()
    .Handler(handler);
```

### Unsubscribe
The Handler-function returns an `IMessageBusSubscription` that inherits from `IDisposable`. To unsubscribe, call `IMessageBusSubscription.Dispose()`.

```csharp
var subscription = bus
    .Subscribe()
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });

...

subscription.Dispose();
```
You can also use a SubscriptionWrapper that unsubscribes when it goes out of scope.
```csharp
public class HandlerClass
{
    private readonly SubscriptionWrapper wrapper;

    HandlerClass(IMessageSubscriptions<ExampleMessage> bus)
    {
        this.wrapper = bus
            .Subscribe()
            .Handler(async message =>
                {
                    await this.SomeMethodAsync();
                    Console.WriteLine(message.Id);
                })
            .Wrapper();
	}}
```

### Using `.Factory()` to instantiate a handler for each message
Note! The handler in this example implements IDisposable, but it is not a requirement. When using a factory to instantiate an IDisposable type, the type is automatically disposed when the message has been handled (unless you specify neverDispose:true).
This approach can come in handy and simplify your code if, for example, your handler class use resources that can can only be used for a short period of time.

#### Overloads
```csharp
.Factory<THandler>(Func<THandler> handlerFactory, bool neverDispose = false);
```
* `handlerFactory` is the method that returns the factory
* `neverDispose` set this to true to prevent the `.Factory()` to dispose the handler after each message. This is for situations when for example you want to wait until the messages start comming before you instantiate the handler. 

#### Example
```csharp
internal class ReadmeFactoryHandler : IMessageHandler<ExampleMessage>, IDisposable
{
    public void Dispose()
    {
        // If the type implements IDisposable, the Dispose method is called as soon as the HandleMessageAsync is done
    }

    public async Task HandleMessageAsync(ExampleMessage message)
    {
        // Do something with the message
    }
}

internal class ReadmeFactoryHandlerSetup
{
    public void SetupSubscription(IMessageBusSubscriptions<ExampleMessage> bus)
    {
        bus
            .Subscribe()
            .Factory(() => new ReadmeFactoryHandler());
    }
}
```

#### Using dependency injection to resolve message handlers
You can easily have your favorite dependency injection container produce the handler instance. 

##### Resolving with ASP.NET Core dependency injection
Note! There is a passage later in this document about how to use Serpent.Common.MessageBus with ASP.NET Core.

###### Registering the bus and the sample service
```csharp
using Serpent.Common.MessageBus;
using Serpent.Common.MessageBus.Extras;

public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();

    // Register message bus for all types
    services.AddSingleton(typeof(IMessageBus<>), typeof(ConcurrentMessageBus<>));

    // These two are required if you want to be able to resolve IMessageBusPublisher<> and IMessageBusSubscriptions
    services.AddSingleton(typeof(IMessageBusPublisher<>), typeof(PublisherBridge<>));
    services.AddSingleton(typeof(IMessageBusSubscriptions<>), typeof(SubscriptionsBridge<>));

    // Register the ReadmeService based on service type
    services.AddSingleton<ReadmeService>();

    // Register the service based on message handler
    services.AddSingleton<ReadmeService, IMessageHandler<ReadmeMessage>>();
}
```
###### Resolving a handler by using the factory method
```csharp
public void SetupSubscriptions(IMesssageBusSubscriptions<ReadmeMessage> bus, IServiceProvider services)
{
    bus
        .Subscribe()
        .Factory(() => services.GetService<ReadmeService>());

    // Using IMessageHandler<>

    bus
        .Subscribe()
        .Factory(() => services.GetService<IMessageHandler<ReadmeMessage>>());
}
```
##### Resolving handlers with Autofac
###### Register the generic bus and the sample handler
```csharp
public void ConfigureServices(IRegistrationBuilder builder)
{
    // Register message busses for all types
    builder
        .RegisterGeneric(typeof(ConcurrentMessageBus<>))
        .As(typeof(IMessageBusPublisher<>))
        .As(typeof(IMessageBusSubscriptions<>))
            .SingleInstance();

    // Register the ReadmeService based on service type and IMessageHandler<>
    builder
        .RegisterType<ReadmeService>()
            .As<ReadmeService>()
            .As<IMessageHandler<ReadmeService>>();
}
```
###### Resolving a handler by using the factory method
```csharp
public void SetupSubscriptions(IMesssageBusSubscriptions<ReadmeMessage> bus, IComponentContext services)
{
    bus
        .Subscribe()
        .Factory(() => services.Resolve<ReadmeService>());

    // Using IMessageHandler<>

    bus
        .Subscribe()
        .Factory(() => services.Resolve<IMessageHandler<ReadmeMessage>>());
}
```

### Approaches to using a message bus
There are a lot of approaches to using a messages bus. I've picked a few that you can use, either only one, several or all in your applications.

* Event/notification
* Workflow
* Request/Response

#### Event/notification
This is probably the simplest way to use a message bus. 

Some example events:
* Data is updated (for example, in a server application, data can be sent to connected web sockets or in a client application, the UI can be refreshed)
* A button was pressed, or the user has dragged and dropped files in a window

In a solution i worked with a few years back, we sent an event message when data in an object in memory was updated and the event handler was responsible for presisting the data to a database.

##### Event/notification message naming
Let the name of the message describe what has happend, instead of what you want to happen (which is workflow or request/response)
Examples:
* UserUpdatedMessage or UserUpdatedEvent
* FilesDroppedInMainWindowMessage or FilesDroppedInMainWindowEvent

#### Workflow
You can create a workflow and publishing a single message starts the workflow. The workflow can be a chain or a tree of messages and message handlers 

A simple cloud file backup workflow example:
1. Message: `StartFileBackupWorkflowMessage`. The message is just the entry point of the workflow. The handler just publishes a `CheckForNewFilesToBackupMessage`.
2. Message: `CheckForNewFilesToBackupMessage`. The handler checks for files modified or created since last check and publishes a `SendBackupFileToCloudStorageMessage` for each of the files.
3. Message: `SendBackupFileToCloudStorageMessage`. The handler sends the file to your favourite cloud storage and posts a `TweetAboutTheBackupMessage`.
4. Message: `TweetAboutTheBackupMessage`. The handler tweets about backing up your file.

For simple workflows like this you may want to merge step 1 & 2.

Without using decorators (you will know about the decorators very soon), this is just an awkward way to write code. 

Usually, you will want to implement the services used by the workflow the traditional way (or using their own workflows). 
For example, instead of using `System.Net.Http.HttpClient` to call a web api, create interface and implementation and have the logic there.

Here is what the workflow may look like in code. Notice that I use .NET's `IServiceProvider`, used in ASP.NET core for resolving instances.
The workflow below will backup all files updated since last transfer files concurrently. We have created parallelism.

```csharp
public class StartFileBackupWorkflowMessage
{
}

public class CheckForNewFilesToBackupMessage
{
}

public class SendBackupFileToCloudStorageMessage
{
    public string Filename { get; set; }
}

public class TweetAboutTheBackupMessage
{
    public string Filename { get; set; }
}

// These are in the Serpent.Common.MessageBus nuget package already. They are only here for clarity
public static class ServiceProviderExtensions
{
    public static IMessageBusPublisher<TMessageType> Publisher<TMessageType>(this IServiceProvider serviceProvider) => serviceProvider.GetService<IMessageBusPublisher<TMessageType>>();
    public static void Publish<TMessageType>(this IServiceProvider serviceProvider, TMessageType message) => serviceProvider.Publisher<TMessageType>().Publish(message);
    public static void PublishRange<TMessageType>(this IServiceProvider serviceProvider, IEnumerable<TMessageType> messages) => serviceProvider.Publisher<TMessageType>().PublishRange(messages);
    public static IMessageBusSubscriptions<TMessageType> Subscriptions<TMessageType>(this IServiceProvider serviceProvider) => serviceProvider.GetService<IMessageBusSubscriptions<TMessageType>>();
    public static IMessageHandlerChainBuilder<TMessageType> Subscribe<TMessageType>(this IServiceProvider serviceProvider) => serviceProvider.Subscriptions<TMessageType>().Subscribe();
}

public class BackupWorkflowSetup
{
    public void SetupWorkflow(
        IServiceProvider services, 
        IGetUpdatedfilesService getUpdatedFilesService, 
        ISendFileToCloudStorageService sendFileToCloudStorageService, 
        ITwitterApi twitterApi)
    {
        // Step #1
        var startWorkflowSubscription = 
            services.Subscribe<StartFileBackupWorkflowMessage>()
                    .Handler(message => services.Publish(new CheckForNewFilesToBackupMessage()));

        // Step #2
        var checkForBackupSubscription =
            services.Subscribe<CheckForNewFilesToBackupMessage>()
                .Handler(async message => 
                    {
                        var files = await getUpdatedFilesService.GetUpdatedFilesAsync();
                        services.PublishRange(files.Filenames.Select(filename => new SendBackupFileToCloudStorageMessage { Filename = filename } ));
                    });

        // Step #3
        var tweetAboutTheBackupPublisher = services.Publisher<TweetAboutTheBackupMessage>();
                
        var sendBackupFileToCloudStorageSubscription =
            services.Subscribe<SendBackupFileToCloudStorageMessage>()
                .Handler(async message => 
                    {
                        await sendFileToCloudStorageService.SendFileToMyCloud(message.Filename);
                        tweetAboutTheBackupPublisher.Publish(new TweetAboutTheBackupMessage { Filename = message.Filename });
                    });

        // Step #4
        var tweetAboutTheBackupSubscription =
            services.Subscribe<TweetAboutTheBackupMessage>()
                .Handler(async message => 
                    {
                        await twitterApi.Tweet("We've just backed up " + message.Filename);
                    });
    }
}



```
Assuming there are actual implementations behind `IGetNewfilenamesService`, `ISendFileToCloudStorageService` and `ITwitterApi`, we've gotten ourselves a little workflow.
Posting a `StartFileBackupWorkflowMessage` to the bus starts the workflow. If there are 5000 new/updated files, 5000 messages are posted to the `SendBackupFileToCloudStorageMessage` bus, and 5000 files are concurrently sent to the cloud storage.
Usually, a degree of parallelism is good in situations like this, but sending 5000 files simultaneously may introduce all sorts of problems, like your cloud provider blocking you or the high level of concurrency ruins your system's performance. 

The decorators I promissed you to write about can do some magic for us. Let's use a decorator to set the fixed (maximum) level of concurrency for step #3 to 16 simulataneous files being backed up.
```csharp
 // Step #3
var sendBackupFileToCloudStorageSubscription =
    services
        .Subscribe<SendBackupFileToCloudStorageMessage>()
            .Concurrent(16)
            .Handler(async message => 
                {
                    await sendFileToCloudStorageService.SendFileToMyCloud(message.Filename);
                    tweetAboutTheBackupPublisher.Publish(new TweetAboutTheBackupMessage { Filename = message.Filename });
                });
```

The same goes for our tweeting, but our twitter api allows only 2 simultaneous tweets. I'm sure the Twitter API allows a lot more, but this is for the sake of the example.
```csharp
// Step #4
var tweetAboutTheBackupSubscription =
    services
        .Subscribe<TweetAboutTheBackupMessage>()
            .Concurrent(2)
            .Handler(async message => 
                {
                    await twitterApi.Tweet("We've just backed up " + message.Filename);
                });
```
Notice that unless the Twitter API is really fast, TweetAboutTheBackupMessage messages are queued inside `.Concurrent(2)` and processed by two worker tasks. 
This is actually pretty cool, that the workflow will allow the faster parts of your workflow to do as much as they can as quickly as they can, while the slower do things at their own pace.
You may want to set the level of concurrency of the different parts of your workflow from configuration. 

That was the `.Concurrent()` decorator. Now, let's add some retry functionality for both the cloud backup and the tweeting, if the services throws exceptions.
```csharp
// Step #3
var sendBackupFileToCloudStorageSubscription =
    services
        .Subscribe<SendBackupFileToCloudStorageMessage>()
            .Retry(
                5, 
                TimeSpan.FromMinutes(1),
                (msg, exception, attempt, maxAttempts, delay, cancellationToken) => {
                        Console.WriteLine("Sending " + msg.Filename + " to cloud backup, attempt " + attempt + " of " + maxattempts + " failed. " + ((attempt != maxAttempts) ? "Will retry in " + delay : "No more retries.") + " Exception: " + exception.ToString());
                    },
                (msg, exception, attempt, maxAttempts, delay) => {
                        Console.WriteLine("Sending " + msg.Filename + " to cloud backup, attempt " + attempt + " of " + maxattempts + " succeeded.");
                    },
                )
            .Concurrent(16)
            .Handler(async message => 
                {
                    await sendFileToCloudStorageService.SendFileToMyCloud(message.Filename);
                    tweetAboutTheBackupPublisher.Publish(new TweetAboutTheBackupMessage { Filename = message.Filename });
                });
   
// Step #4
var tweetAboutTheBackupSubscription =
    services
        .Subscribe<TweetAboutTheBackupMessage>()
            .Retry(3, TimeSpan.FromSeconds(10))
            .Concurrent(2)
            .Handler(async message => 
                {
                    await twitterApi.Tweet("We've just backed up " + message.Filename);
                });
```
When sending the file to cloud backup, we want to make 5 attempts, with 1 minute waiting between them and log any errors. When tweeting, we just want 3 retries and no logging.
As you have noticed already, we put `.Retry()` before `.Concurrent()`. It does not have to be that way. If we put `.Retry()` before `.Concurrent()`, every attempt is queued and handled as soon as the `.Concurrent()` workers can. 
If we put `.Retry()` after `.Concurrent()`, the `.Concurrent()` worker that handled the message that threw the exception is busy waiting to retry for a full minute before retrying. It's based on your requirements.

We can use a decorator more than once to make the functionality even more advanced. Let's put `.Retry()` both before and after `.Concurrent()` and add some more logging:
```csharp
// Step #3
var sendBackupFileToCloudStorageSubscription =
    services
        .Subscribe<SendBackupFileToCloudStorageMessage>()
            .Exception((message, exception) => Console.WriteLine("All attempts failed to send " + messge.Filename + "!" + exception));
            .Filter(
                beforeMessage => Console.WriteLine("Sending " + beforeMessage.Filename + " to cloud"),
                afterMessage => Console.WriteLine(afterMessage.Filename + " sent successfully to cloud"))    
            .Retry(
                5, 
                TimeSpan.FromMinutes(1),
                (msg, exception, attempt, maxAttempts, delay, cancellationToken) => {
                        Console.WriteLine("Sending " + msg.Filename + " to cloud backup, attempt " + attempt + " of " + maxattempts + " failed. " + ((attempt != maxAttempts) ? "Will retry in " + delay : "No more retries.") + " Exception: " + exception.ToString());
                    },
                (msg, exception, attempt, maxAttempts, delay) => {
                        Console.WriteLine("Sending " + msg.Filename + " to cloud backup, attempt " + attempt + " of " + maxattempts + " succeeded.");
                    },
                )
            .Concurrent(16)
            .Retry(2, TimeSpan.FromSeconds(1))
            .Handler(async message => 
                {
                    await sendFileToCloudStorageService.SendFileToMyCloud(message.Filename);
                    tweetAboutTheBackupPublisher.Publish(new TweetAboutTheBackupMessage { Filename = message.Filename });
                });
```
Now, if an exception is thrown in the handler, the call will be retried after 1 second, and if that fails as well, it will be retried a minute later. 

There are quite a few out of the box decorators and a whole chapter in this readme that describes them. It's pretty neat to be able to add cross cutting concerns to all message handlers with just a line of code.

##### Workflow message naming
I suggest the name of your workflow messages describe what the step does. Not in detail but what you want to happen. A workflow can also be based entirely on events/notifications. Be warned though, my experience of this is that it's a lot less intuitive and harder to debug and follow the workflow. Especially when using message handlers in separate classes.

#### Request/response service

##### Request messsage/response message
You send a request message to invoke the service and wait for a response message to get the response / status.
Someone who wants to use your service, publishes a Request message, and then waits for a response message.

```csharp
public void Setup(IMessageSubscriptions<MyRequestMessage> requestBus, IMessageBusPublisher<MyResponseMessage> responseBus)
{
    var subscription = 
        requestBus.Subscribe()
            .NoDuplicates(message => message.UserId)
            .Concurrent(20)
            .Handler(async (message, token) => 
            {
                // Message received, do something and reply
                responseBus.Publish(new MyResponseMessage());
            });
}
```

Advantages:
* Multiple requests can translate into a single handler call
* Reusability in other parts of the application and other projects

Disadvantages:
* Added level of complexity

##### Request message/response func

### Anti patterns
Anti patterns are suboptimal patterns that we are more or less likely to end up in. 

#### Anti Pattern: Mixing names and approaches
Let's say we have the following workflow:

1. Get file data
2. Call API to send data / get response
3. Write feedback file
4. Delete the original file

It's easy to start mixing Request/Response and Workflow approaches as one, which can lead to a lot of complexity.

Example:

1. `GetMyFileDataRequestMessage` - The `GetMyFileDataRequestMessageHandler` subscribes to `GetMyFileDataRequestMessage` messages and reads the file from disk. Responds with `GetMyFileDataResponseMessage`. So far so good.
2. `GetMyFileDataResponseMessageHandler` subscribes to `GetMyFileDataResponseMessage` messages and calls the API to send the data. When done, it publishes a `SendMyDataToApiResponseMessage`.
3. `SendMyDataToApiResponseMessageHandler` writes the feedback file and publishes a `WriteFeedbackFileResponseMessage`.
...

It's very unintuitive that `GetMyFileDataResponseMessageHandler` is the one calling the API, and we can't really reuse the solution. In addition to this, it's hard to extend and follow the execution flow.

Instead, we can either create 4 micro services with the Request/Response approach and join them with a workflow.



### The message handler chain (MHC)
MHC (Message Handler Chain) is the execution tree where messages pass through. We can easily use this concept to add functionality that normally might require quite some time to write yourself.
The MHC exists both on the subscriber and publisher side.

When a subscription receives a message, it passes through the MHC before it reaches the `.Handler()` or `.Factory()` *MHCD (MHC Decorator)* which is the last step of the message handler chain.
Example
```csharp
var subscription = bus
    .Subscribe()
    // Insert decorators here!
    .Handler(async message =>
        {
            using (var fileStream = File.OpenRead(message.Filename))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    var fileText = await streamReader.ReadToEndAsync();
                    if (fileText == "Warning")
                    {
                        // Do something
                    }
                }
            }
        });
```

1. The subscription receives the message
2. The `.Handler()` calls our specified handler.

Let's add some functionality to our subscription!

```csharp
var subscription = bus
    .Subscribe()
    // Inserted decorators
    .NoDuplicates(message => message.Filename)
    .Delay(TimeSpan.FromSeconds(5))
    .Retry(5, TimeSpan.FromSeconds(30))
    .Concurrent(16)
    .LimitedThroughput(100, TimeSpan.FromMilliseconds(100))
    // The handler
    .Handler(async message =>
        {
            using (var fileStream = File.OpenRead(message.Filename))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    var fileText = await streamReader.ReadToEndAsync();
                    if (fileText == "Danger! Danger! High voltage!")
                    {
                        // Do something
                    }
                }
            }
        });
```
Let's break down what happens here
1. The subscription receives the message
2. `.NoDuplicates()` stops/drops all messages that are already being handled by the subscription, making sure the same file is not read more than once concurrently 
3. `.Delay()` delay the handling of all messages by 5 seconds. In this case, to make sure the "other" system creating the files is done writing before we start reading.
4. `.Retry()` make a total of 5 attempts to read the file. If the handler throws an exception, `.Retry()` will wait 30 seconds before trying again
5. `.Concurrent()` processes up to 16 files concurrently, queueing any excess
6. `.LimitedThroughput()` limits throughput to maximum of 100 messages per 100ms = 1000 messages/second 
7. `.Handler()` calls our code reading the file

We added functionality to our subscription MHC by adding MHC decorators.  

### Message handler chain decorators (MHCD's)
Message handler chain decorators (MHCD's) are functionality you can add to your MHC (subscription or bus).

* The decorators are executed in the order they are specified.
* Exceptions thrown in a message handler are propagated back in the reverse order they are specified. 
* Exceptions and awaitability do not pass `.FireAndForget()`, `.SoftFireAndForget()`, `.ConcurrentFireAndForget()`, and `.LimitedThroughputFireAndForget()`.
* A decorator can be added more than once to the message chain to further customize functionality. 
* The decorators can be applied both to your subscriptions and directly to the bus.

Note! The message handler chain and it's decorators can also be used stand alone, without a bus. See examples later.

Using the decorators when configuring a subscription
```csharp

    var subscription = bus
        .Subscribe()
            .NoDuplicates(message => message.Id)    // Do not allow messages with an Id matching a message already being handled
            .Delay(TimeSpan.FromSeconds(5))         // Delay the message 5 seconds
            .Retry(3, TimeSpan.FromSeconds(60))     // If the two attempts in the next line fail, try again 2 more times (3 attempts)
            .Retry(2, TimeSpan.FromSeconds(5))      // If the handler fails, retry once (two attempts in total if the first fails)
                .Handler(async message =>
                    {
                        await this.SomeMethodAsync();
                        Console.WriteLine(message.Id);
                        throw new Exception("Fail!");
                    });
```
#### Message handler chain decorators reference
To customize the way your subscription is handled (or the way the bus publishes messages), you can add one or more decorators. 

Here's a summary of the currently available decorators. If you have requirements that these decorators does not support, you can write your own decorators (see the chapter Custom MHC Decorators). 

* `.Append()` - Append a message for each message. Like LINQ `.Append()`.
* `.AppendMany()` - Append a range of messages based on an incoming message. Supports recursive unwrapping of trees and such.
* `.Branch()` - Split the MHC into two or more parallel trees. 
* `.BranchOut()` - Branch the MHC tree into one or more MHC trees parallel to the normal MHC tree.
* `.Cast()` - Cast each message to a specific type.
* `.Concurrent()` - Parallelize and handle X concurrent messages.
* `.ConcurrentFireAndForget()` - Parallelize and handle X concurrent messages but does not provide delivery feedback and does not pass through exceptions.
* `.Delay()` - Delay the execution of the message handler.
* `.DispatchOnTaskScheduler()` - Have the messages dispatched on a new Task on a specified Task Scheduler. For example, to have all messages handled by the UI thread.
* `.DispatchOnCurrentContext()` - Have the messages dispatched to a new Task on the current Task Scheduler. For example, to have all messages handled by the UI thread.
* `.Distinct()` - Only pass through unique messages based on key.
* `.Exception()` - Handle exceptions not handled by the message handler.
* `.Filter()` - Execute a method before and after the execution of the message handler. Can also filter messages to stop the message from being processed further.
* `.FireAndForget()` - Spawn a new Task to execute each message.
* `.First()` - Only let 1 message pass through. Optionally based by a predicate (which is the same as `.Where().First()`).
* `.LimitedThroughput()` - Limit the throughput to X messages per period. For example, 100 messages per second. Or 10 messages per 100 ms.
* `.LimitedThroughputFireAndForget()` - Same as `.LimitedThroughput()` but break the feedback chain.
* `.NoDuplicates()` - Drop all duplicate messages by key. Duplicate messages are dropped.
* `.OfType()` - Only pass on messages of a certain type. 
* `.Prepend()` - Prepend a message for each message handled. Like LINQ `.Prepend()`.
* `.Retry()` - Retry after TimeSpan, X times to deliver a message if the message handler fails (throws an exception)
* `.Select()` - Change message message type for the remaining message handler chain. Like LINQ `.Select()`.
* `.SelectMany()` - Change message message type for the remaining message handler chain and extract messages from an enumerable. Like LINQ `.SelectMany()`.
* `.Semaphore()` - Limit the number of concurrent messages being handled by this subscription.
* `.Skip()` - Skip the first X messages. Like LINQ `.Skip()`.
* `.SkipWhile()` - Skip messages as long as the predicate succeeds. Like LINQ `.SkipWhile()`.
* `.SoftFireAndForget()` - Executes the synchronous parts of the next MHCD or Handler, synchronous but everything asynchronous is executed without feedback. 
* `.Take()` - Only let X messages pass through. Like LINQ `.Take()`.
* `.TakeWhile()` - Only let messages pass through as long as a predicate succeeds. The same as `.Where().Take()`. Like LINQ `.TakeWhere()`.
* `.WeakReference()` - Keeps a weak reference to the message handler and unsubscribes when the message handler has been reclaimed by GC
* `.Where()` - Filter messages based on predicate. Like LINQ `.Where()`

Stacking these allow you to configure in a lot of advanced functionality, for example:
```csharp
public class Message
{
 public string Id { get; set; }
 public bool IsPolite { get; set; }
}

bus
    .Subscribe()
    .SoftFireAndForget()                    // Don't let the publisher await no more
    .NoDuplicates(message => message.Id)    // Drop all messages already in the message chain based on key
    .Where(message => message.IsPolite)     // Only allow plite messages to pass through
    .Exception(message => Console.WriteLine("All 10 attempts failed"))  // no exceptions will pass this point
    .Retry(2, TimeSpan.FromSeconds(60)) // If the first 5 attempts fail, wait 60 seconds and try 5 more times
    .Exception(message => { Console.WriteLine("First 5 attempts failed. Retrying in 50 seconds"; return true; })) // true is to have the exception to continue up the chain
    .Retry(5, TimeSpan.FromSeconds(10)) // Try 5 times, with a 10 second delay between failures
    .Concurrent(16) // 16 concurrent handlers
    .Handler(message => Console.WriteLine("I handle all messages"));
```

Some stacking combinations are not so useful, for example:
```csharp
bus
    .Subscribe()
    .Concurrent(16)
    .FireAndForget()
    .Handler(message => Console.WriteLine("I handle all messages"));
```
`.FireAndForget()` will return immediately and render `.Concurrent()` useless since all messages will be spawned on their own tasks immediately.


#### `.Append()`
Appends a message to the subscription right after the current message is handled.
The overloads with the `predicate` parameter are used to conditionally append a message. 
`.Append()` can be used to traverse a single node relation, like a parent node.

##### Overloads
```csharp
.Append(Func<TMessageType, TMessageType> messageSelector);
.Append(Func<TMessageType, TMessageType> messageSelector, Func<TMessageType, bool> predicate, bool isRecursive = false);
.Append(Func<TMessageType, Task<TMessageType>> messageSelector);
.Append(Func<TMessageType, Task<TMessageType>> messageSelector, Func<TMessageType, Task<bool>> predicate, bool isRecursive = false);
```
* `messageSelector` is the selector that returns the message to append.
* `predicate` only append the message if predicate returns true.
* `isRecursive` passes the appended message through the append mechanism, which allows recursion. 

##### Examples
```csharp
public class MyMessage
{
    public string Text { get; set; }
}

IMessageSubscriptions<MyMessage> bus = new ConcurrentMessageBus<MyMessage>();

var subscription = bus
    .Subscribe()
    // Always append a message
    .Append(message => new MyMessage { Text = "Appended" })
    .Handler(async message =>
        {
            Console.WriteLine(message.Text);
        });
```
Using `.Append()` to unwrapp and handle a message tree recursively.
```csharp
public class MyMessage
{
    public int Id { get; set; }
    public MyMessage InnerMessage { get; set; }
}

IMessageSubscriptions<MyMessage> bus = new ConcurrentMessageBus<MyMessage>();

var subscription = bus
    .Subscribe()
    // Unwrapp the inner message, and do it recursively
    .Append(
        message => message.InnerMessage != null, 
        message => message.InnerMessage,
        true)
    .Handler(async message =>
        {
            Console.WriteLine("Handled: " + message.Id);
        });
```

#### `.AppendMany()`
Appends several messages to the subscription right after the current message is handled.
The overloads with the `predicate` parameter are used to conditionally append messages. 
`.AppendMany()` can be used to iterate through a tree of messages, handling each of them.

##### Overloads
```csharp
.AppendMany(Func<TMessageType, IEnumerable<TMessageType>> messageSelector);
.AppendMany(Func<TMessageType, IEnumerable<TMessageType>> messageSelector, Func<TMessageType, bool> predicate, bool isRecursive = false);
.AppendMany(Func<TMessageType, Task<IEnumerable<TMessageType>>> messageSelector);
.AppendMany(Func<TMessageType, Task<IEnumerable<TMessageType>>> messageSelector, Func<TMessageType, Task<bool>> predicate, bool isRecursive = false);
.AppendMany(Func<TMessageType, CancellationToken, Task<IEnumerable<TMessageType>>> messageSelector);
.AppendMany(Func<TMessageType, CancellationToken, Task<IEnumerable<TMessageType>>> messageSelector, Func<TMessageType, CancellationToken, Task<bool>> predicate, bool isRecursive = false);

```
* `messageSelector` is the selector that returns the messages to append.
* `predicate` only append the message if predicate returns true.
* `isRecursive` passes the appended messages through the append mechanism, which allows recursion. 

##### Examples
```csharp
public class MyMessage
{
    public string Text { get; set; }
    public IEnumerable<MyMessage> ChildMessages { get; set; }
}

IMessageSubscriptions<MyMessage> bus = new ConcurrentMessageBus<MyMessage>();

var subscription = bus
    .Subscribe()
    // Always append a message
    .AppendMany(
        message => message.ChildMessages != null,
        message => message.ChildMessages)   // only 1 level, no recursion
    .Handler(async message =>
        {
            Console.WriteLine(message.Text);
        });
```
Using `.AppendMany()` to unwrapp and handle a message tree recursively.
```csharp
public class MyMessage
{
    public int Id { get; set; }
    public IEnumerable<MyMessage> ChildMessages { get; set; }
}

IMessageSubscriptions<MyMessage> bus = new ConcurrentMessageBus<MyMessage>();

var subscription = bus
    .Subscribe()
    // Unwrapp the inner message, and do it recursively
    .AppendMany(
        message => message.ChildMessages != null, 
        message => message.ChildMessages,
        true)   // recursion - unwrapp the entire tree and handle each node
    .Handler(async message =>
        {
            Console.WriteLine(message.Id);
        });
```
#### `.Branch()`
`.Branch()` is not actually a decorator, like `.BranchOut()`, but a handler that splits the message handler chain into two ore more message handler chains.
The branches are invoked "softly parallel", which means that if the first branch does only CPU intensive work for seconds, the other branches will not get their message delivered until the first branch is done. If the first branch awaits I/O, the next branch will start, and so on.
The feedback chain is intact through Branch, so if one of the branches throws an exception which is not handled, it is passed up the chain and can potentially prevent other branches to complete. If there is no FireAndForget, the publisher is able to await the delivery of a message to all branches.

##### Overloads
```csharp
.Branch(
    Action<IMessageHandlerChainBuilder<TMessageType>> firstBranch,
    params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches);
```
* `firstBranch` - the first branch - this value must not be null.
* `branches` - additional branches
##### Examples
```csharp
bus
    .Subscribe()
    .NoDuplicates(message => message.Id)
    .Branch(
        branch1 => branch1
            .Delay(TimeSpan.FromSeconds(1))
            .Handler(message => Console.WriteLine("Invoked 1 second after the second branch")),
        branch2 => branch2
            .Handler(message => Console.WriteLine("Invoked immediately"))
    );
```
You can have as many branches as you like and branches in branches in branches...
```csharp
bus
    .Subscribe()
    .NoDuplicates(message => message.Id)
    .Branch(
        branch1 => branch1
            .Delay(TimeSpan.FromSeconds(1))
            .Handler(message => Console.WriteLine("Branch 1")),
        branch2 => branch2
            .Delay(TimeSpan.FromSeconds(2))
            .Handler(message => Console.WriteLine("Branch 2")),
        branch3 => branch3
            .Delay(TimeSpan.FromSeconds(3))
            .Handler(message => Console.WriteLine("Branch 3")),
        branch4 => branch4
            .Delay(TimeSpan.FromSeconds(4))
            .Handler(message => Console.WriteLine("Branch 4")),
        branch5 => branch5
            .Delay(TimeSpan.FromSeconds(5))
            .Handler(message => Console.WriteLine("Branch 5")),
        branch6 => branch6
            .Delay(TimeSpan.FromSeconds(6))
            .Handler(message => Console.WriteLine("Branch 6")),

        branchInBranch => branchInBranch
            .Delay(TimeSpan.FromSeconds(10))
            .Branch(
                subSubBranch1 => 
                    .Handler(message => Console.WriteLine("Branch 10.1")),
                subSubBranch2 => 
                    .Handler(message => Console.WriteLine("Branch 10.2"))
                ));
```


#### `.BranchOut()`
`.BranchOut()` works like `.Branch()` but adds one or more parallel MHC trees to the current tree instead of splitting the current tree into one or more trees. 

##### Overloads
```csharp
.BranchOut(params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches);
```

##### Examples
```csharp
bus
    .Subscribe()
    .NoDuplicates(message => message.Id)
    .BranchOut(
        branch => branch
            .Delay(TimeSpan.FromSeconds(10))
            .Filter(message => message.Id == "Message 2")
            .Handler(message => Console.WriteLine("I only handle Message 2")))
    .Handler(message => Console.WriteLine("I handle all messages"));
```
The subscription will not handle any concurrent duplicates and is branched to two separate message handlers.
Message 2 will be handled by the branched message handler 10 seconds after the subscription receives it while the normal handler will handle all messages immediately.

#### `.Concurrent()`
Parallelizes the message handler chain to a maximum of X concurrent messages. Messages that arrive to `.Concurrent()` are queued and handled as soon as possible, in a FIFO (first in first out) manner. 
`.Concurrent()` both parallelizes and limits parallelism to your defined level. If you use `.Concurrent(20)` to send e-mails, up to 20 and no more than 20 e-mails are sent concurrently.

You usually get a lot higher throughput by handling multiple concurrent messages, especially when the handlers perform I/O (read/write to disk, sending e-mails, calling web services, web apis, sending data to an Azure service bus, etc.).
Often there is also a threshold where too much parallelism degrades performance. Sending 20 e-mails concurrently is likely much faster than 1, but sending 100 e-mails concurrently may put too much pressure on the SMTP server and can be slower than 20.
It's often wise to make the level of parallelism configurable. You may experience great performance at some level on your developer machine, and a different one when you deploy to your test and production environments.

`.Concurrent()` will keep the feedback chain which will allow the caller to await until the message is handled. Exceptions thrown pass right back to the caller. 
This makes decorators like `.Retry()` work properly both before and after `.Concurrent()`. See the examples.

If you do not need to maintain the feedback chain, you can use `.ConcurrentFireAndForget()`, which has slightly smaller execution footprint (it's a little faster).

##### Overloads
```csharp
.Concurrent(int maxNumberOfConcurrentMessages);
```

##### Attribute wireup
```csharp
[Concurrent(maxNumberOfConcurrentMessages)]
```

##### Examples
Let's say you want to send a newsletter to a 5000 of recipients. If you don't parallelize the process, it may take quite a while. Let's parallelize it.

```csharp
var smtpClient = new SmtpClient();

var subscription = bus
    .Subscribe()
    .Concurrent(20)	
    .Handler(async message =>
        {
            await smtpClient.SendMailAsync(
                new MailMessage(
                    "noreply@mynewsletter.test", 
                    message.EmailAddress,
                    "Your daily news",
                    "This is the news letter content"));
        });
```

##### `.Retry()` stacked with `.Concurrent()`
We can specify `.Retry()` before `.Concurrent()` to process other messages while a failed attempt is waiting to retry, or `.Retry()` after `.Concurrent()` to lock upp one of the 20 concurrent handlers for upp to 4 minutes waiting for a retry.

```csharp
var smtpClient = new SmtpClient();

var subscription = bus
    .Subscribe()
    // Handle errors from last attempt
    .Exception(message => Console.WriteLine("Mail failed id:" + message.Id + " failed all attempts"))
    // Try 5 times. First retry (out of 4) after 1 minute
    .Retry(5, TimeSpan.FromMinutes(1))
    // 16 concurrent messages being handled
    .Concurrent(16)	
    .Handler(async message =>
        {
            await smtpClient.SendMailAsync(
                new MailMessage(
                    "noreply@mynewsletter.test", 
                    message.EmailAddress,
                    "Your daily news",
                    "This is the news letter content"));
        });
```

#### `.ConcurrentFireAndForget()`
`.ConcurrentFireAndForget()` is roughly the same as `.SoftFireAndForget().Concurrent()` - feedback can't pass through, but for scenarios where performance is very important, `.ConcurrentFireAndForget()` has smaller overhead.

##### Overloads
```csharp
.ConcurrentFireAndForget(int maxNumberOfConcurrentMessages);
```

##### Example
```csharp

var subscription = bus
    .Subscribe()
    .ConcurrentFireAndForget(16)
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });
```

#### `.Delay()`
Delay the message handler chain a specified period. This can be useful when you want the message handler to be invoked some time after the message was sent, for example, if you want to process a file after it's written to a path that you monitor, and you want the process writing the file to be able to finish before you start processing it.

##### Overloads
```csharp
.Delay(TimeSpan timeToWait);
.Delay(int timeInMillisecondsToWait);
```
Delay (await) the specified number of milliseconds

##### Example
```csharp
var subscription = bus
    .Subscribe()
    .Delay(TimeSpan.FromSeconds(10)) // delay the message 10 seconds
    .Handler(async message =>
        {
            await this.SomeMethodAsyncInvoked10SecondsLaterAsync();
            Console.WriteLine(message.Id);
        });
```


```csharp
var subscription = bus
    .Subscribe()
    .Delay(10000)) // delay 100000 milliseconds before handling the message
    .Handler(async message =>
        {
            await this.SomeMethodAsyncInvoked10SecondsLaterAsync();
            Console.WriteLine(message.Id);
        });
```

#### `.DispatchOnCurrentContext()`
Invoke all calls on the current Task Scheduler. This can come in handy if you use the message bus to handle messages on a UI where it's important that the messages are invoked on the UI thread.
Use this method when initializing the message handler chain on the UI thread (or other context).

##### Overloads
```csharp
```
##### Examples
```csharp
```

#### `.DispatchOnTaskScheduler()`
Invoke all calls on a specified Task Scheduler. This can come in handy if you use the message bus to handle messages on a UI where it's important that the messages are invoked on the UI thread.

##### Overloads
```csharp
```
##### Examples
```csharp
```

#### `.Distinct()`
Only allow a message with a certain key to be handled once.
##### Overloads
```csharp
.Distinct(Func<TMessageType, TKeyType> keySelector);
.Distinct(Func<TMessageType, TKeyType> keySelector, IEqualityComparer<TKeyType> equalityComparer);
.Distinct(Func<TMessageType, Task<TKeyType>> keySelector);
.Distinct(Func<TMessageType, Task<TKeyType>> keySelector, IEqualityComparer<TKeyType> equalityComparer);
.Distinct(Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector);
.Distinct(Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector, IEqualityComparer<TKeyType> equalityComparer);
```
* `keySelector` - the function returing the key
* `equalityComparer` - an equality comparer for the key. Can for example be StringComparer.OrdinalIgnoreCase or your own homegrown.

The key can be the message itself.

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .Distinct(message => message.Id)    // Only allow a message with a certain key to be delivered once
    .Handler(async message =>
        {
            await this.SomeMethodAsyncInvoked10SecondsLaterAsync();
            Console.WriteLine(message.Id);
        });
```
Async with CancellationToken
```csharp
var subscription = bus
    .Subscribe()
    .Distinct(async (message, token) => GetKeyAsync(message, token))    // Only allow a message with a certain key to be delivered once
    .Handler(async message =>
        {
            await this.SomeMethodAsyncInvoked10SecondsLaterAsync();
            Console.WriteLine(message.Id);
        });
```

#### `.Exception()`
`Exception()` invokes a method if the message handler (or a MHC Decorator below in the chain) throws an exception. 
If the exception handler returns nothing or false, the exception is caught and does not propagate further up the chain.
If you want the exception to continue it's journey up the chain, for exampel to use the `Retry()` decorator, return true.

This decorator can for example be useful for logging exceptions or trigger something else to happen as a response. 

##### Overloads
```csharp
.Exception<TMessageType>(Action<TMessageType, Exception> exceptionHandlerAction);
.Exception<TMessageType>(Func<TMessageType, Exception, bool> exceptionHandlerFunc);
.Exception<TMessageType>(Func<TMessageType, Exception, Task<bool>> exceptionHandlerFunc);
.Exception<TMessageType>(Func<TMessageType, Exception, Task> exceptionHandlerFunc);
.Exception<TMessageType>(Func<TMessageType, Exception, CancellationToken, Task<bool>> exceptionHandlerFunc);
.Exception<TMessageType>(Func<TMessageType, Exception, CancellationToken, Task> exceptionHandlerFunc);
```

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .Exception(
        (message, exception) =>
        {
            Console.WriteLine("Error! " + exception));
            return true; // Propagate the exception up the chain
        })
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });

// And of course, you can have the message handled by a method instead

var subscription = bus
    .Subscribe()
    .Exception(this.ErrorHandlerAsync)
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });

public async Task ErrorHandlerAsync(ExampleMessage message, Exception exception)
{
    switch (exception)
    {
        case ArgumentNullException argumentNullException:
			// Handle this error
			Console.WriteLine("Argument null!! " + exception));
            break;
		default:
			// All other errors
			Console.WriteLine("Error! " + exception));
			await DoSomeAsyncErrorHandlingStuff();
    }

}

public async Task HandleMessageAsync(ExampleMessage message)
{
	throw new Exception("FAIL");
}
..

var subscription = bus
    .Subscribe()
	.Exception(this.ErrorHandlerAsync)
    .Handler(this.HandleMessageAsync);

```

Stack it with .`Filter()`
```csharp
 var subscription = bus
    .Subscribe()
    .Exception(
    (message, exception) =>
    {
        Console.WriteLine("Exception: " + exception);
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
```
#### `.Filter()`
`.Filter()` executes a function before the message handler (or the next subscription modifier) is executed. 
The filter function can optionally return false to drop the message, or true to let it pass through.
`.Filter()` can also execute a function after the message is handled successfully. To catch exceptions thrown, use `.Exception()`.
`.Filter()` can be a sweet way to add logging, together with `.Exception()`.
The last overload of `.Filter()` is an inline decorator. To keep the chain you have to call the message handler passed as the second parameter.
##### Overloads
```csharp
.Filter(Action<TMessageType> beforeInvoke = null, Action<TMessageType> afterInvoke = null);
.Filter(Func<TMessageType, bool> beforeInvoke = null, Action<TMessageType> afterInvoke = null);
.Filter(Func<TMessageType, Task<bool>> beforeInvoke = null, Func<TMessageType, Task> afterInvoke = null);
.Filter(Func<TMessageType, CancellationToken, Task<bool>> beforeInvoke = null, Func<TMessageType, CancellationToken,Task> afterInvoke = null);
```
* `beforeInvoke` - action or func called before the method is invoked. Some overloads allow returning a boolean. Return `true` or use an overload that does not return a value to always call the inner decorator, or return `false` to prevent the next decorator from executing..  
* `afterInvoke` - action or func called after the method is invoked. THIS METHOD IS NOT CALLED IF THE HANDLER THROWS AN EXCEPTION.

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .Filter(message => message.Id == "One") // only keep messages with Id == "One"
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });
```
Before and after the message handler
```csharp
var subscription = bus
    .Subscribe()
    .Filter(
        message => Console.WriteLine("Before the message handler is invoked"),
        message => Console.WriteLine("The message handler succeeded as far as we know"))
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });
```
Async
```csharp
var subscription = bus
    .Subscribe()
    .Filter(
        async message => 
        {
            Console.WriteLine("Before the message handler is invoked");
            await SomethingAsync();
            return false; // Stop the message here
        },
        message => Console.WriteLine("The message handler succeeded as far as we know"))
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });
```
Async with cancellation token
```csharp
var subscription = bus
    .Subscribe()
    .Filter(
        async (message, token) => 
        {
            Console.WriteLine("Before the message handler is invoked");

            token.ThrowIfCancellationRequested();
            // or
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("Cancelled!");
                return false; // Stop the message, right here
            }

            return true; 
        },
        message => Console.WriteLine("The message handler succeeded as far as we know"))
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });
```



#### `.FireAndForget()`
NOTE! `.FireAndForget` should be avoided if possible. `.SoftFireAndForget()` is a much better alternative in most cases.
By default, messages are dispatched through the MHC to the handler function. If a decorator or the message handler take 10 seconds to complete, control is not returned to the decorators and ultimately the publisher for 10 seconds. 
You can use `.FireAndForget()` to invoke the next MHC decorator or the message handler as a new Task.
Since a new task is started for each message, this subscription handler will introduce infinite concurrency, which means, 
if 30 000 messages are published to the bus, 30 000 tasks are started which will (most likely) totally ruin your applications performance. 
*Using* `.SoftFireAndForget()` *is often a better option* if you just want to break the feedback of the message handler chain.
Another option would be `.ConcurrentFireAndForget()` or `.SoftFireAndForget()` together with `.Concurrent()`.

##### Overloads
```csharp
.FireAndForget();
```
##### Example
```csharp
var subscription = bus
    .Subscribe()
    .FireAndForget()
    .Handler(async message =>
        {
            Console.WriteLine("Invoked - Fired and forgotten: " + message.Id);
        });
```
#### `.First()`
Pass only a single message through the chain, optionally based on a predicate.

##### Overloads
```csharp
.First();
.First(Func<TMessageType, bool> predicate);
.First(Func<TMessageType, Task<bool> predicate);
.First(Func<TMessageType, CancellationToken, Task<bool> predicate);
```
##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .First()
    .Handler(async message =>
        {
            Console.WriteLine("Invoked only once");
        });
```

`.First()` with a predicate
```csharp
var subscription = bus
    .Subscribe()
    .First(message => message.Id == "2")
    .Handler(async message =>
        {
            Console.WriteLine("Invoked only once, when the first message with Id = 2 comes.");
        });
```

`.First()` with an async predicate
```csharp
var subscription = bus
    .Subscribe()
    .First(async message => 
        {
            return await ValidateMessageAsync(message);
        })
    .Handler(async message =>
        {
            Console.WriteLine("Invoked only once, when ValidateMessagesAsync(message) returns true.");
        });
```

#### `.LimitedThroughput()`
Limits the number of messages passing through the decorator during a specified period.
A new period starts as soon as the previous period ends. If you set the limit to 100 messages per second and 101 messages are sent during a second, the 101th message is queued and handled first in line when the next period starts. 
Messages are not evenly distributed within the period, which means, `.LimitedThroughput(1000, TimeSpan.FromSeconds(1))` will allow all 1000 messages to can pass through during the periods first millisecond, if the rest of the mesasge handler chain is fast enough to handle it.
To distribute messages more evenly, you can make the period smaller - `.LimitedThroughput(100, TimeSpan.FromSeconds(0.1))`. 

`.LimitedThroughput()` count messages as they arrive, which means only X message handlers can be started per period. 

##### Overloads
```csharp
.LimitedThroughput(int maxMessagesPerPeriod, TimeSpan? periodSpan = null);
```
##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .LimitedThroughput(100, TimeSpan.FromSeconds(0.5))
    .Handler(message =>
        {
            Console.WriteLine("Limited edition!");
        });

var subscription = bus
    .Subscribe()
    .LimitedThroughput(200, TimeSpan.FromMilliseconds(1000))
    .Handler(message =>
        {
            Console.WriteLine("Limited edition!");
        });
```

#### `.LimitedThroughputFireAndForget()`
This is roughly the same functionality as `SoftFireAndForget().LimitedThroughput()` but since `.LimitedThroughputFireAndForget()` does not keep the feedback chain, it's has a little less overhead.

##### Overloads
```csharp
.LimitedThroughputFireAndForget(int maxMessagesPerPeriod, TimeSpan? periodSpan = null)
```

#### `.NoDuplicates()`
`.NoDuplicates()` will drop all messages that have the same key as a message being handled by the chain already.
This means if a message enters `.NoDuplicates()` with the same key as another message that has already entered but not exited the same `.NoDuplicates()`, it is dropped.

If you for instance have a message handler that is responsible for deleting a file from disk, the file can only be deleted once, and therefore we can remove duplicates, based on full path and filename.
Another example would be a handler responsible for loading a project from a database. As long as the data is not fully loaded, it would be better to just wait for the first load request to finish.

##### Overloads
```csharp
.NoDuplicates(Func<TMessageType, TKeyType> keySelector);
.NoDuplicates(Func<TMessageType, TKeyType> keySelector, IEqualityComparer<TKeyType> equalityComparer);
```

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .NoDuplicates(message => message.FullPathName)
    .Handler(message =>
        {
            Console.WriteLine("Concurrently unique!");
        });
```

This next example demonstrates using an equality comparer to eliminate duplicates case insensitively.
```csharp
var subscription = bus
    .Subscribe()
    .NoDuplicates(message => message.FullPathName, StringComparer.OrdinalIgnoreCase)
    .Handler(async message =>
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            Console.WriteLine("Concurrently unique, insensitively!");
        });

// This message will be handled
await bus.PublishAsync(new MyMessage("c:\\temp\\filename1"));
// But this won't, since it's a duplicate, and the second the handler takes has not passed
await bus.PublishAsync(new MyMessage("c:\\TEMP\\fileName1"));
```

#### `.Prepend()`
Handle a new message before every incoming message. See Append for more information.
Prepend currently does not support predicate and recursive. There is also no PrependMany.

##### Overloads
```csharp
.Prepend<TMessageType>(Func<TMessageType, Task<TMessageType>> messagePrependFunc)
.Prepend<TMessageType>(Func<TMessageType, TMessageType> messageAppendFunc)
```

#### `.Retry()`
Retries calling the message handler if it fails (throws an exception).
You specify the number of attempts (not retries). If all attempts fail, `Retry()` throws a `Serpent.Common.MessageBus.Exceptions.RetryFailedException` that contains the number of attempts, retry delay and a collection of all exceptions that was thrown.

The exception delay starts after the invokation of an `exceptionFunc` or `exceptionAction`. You can use the exceptionFunc to wait a specific delay by setting `retryDelay` to TimeSpan.Zero and then awaiting `Task.Delay()`.

##### Overloads
```csharp

// Try up to maxNumberOfAttempts if the handler fails. Wait retryDelay between the attempts.
.Retry(
    int maxNumberOfAttempts,
    TimeSpan retryDelay);

// Call an exceptionHandler for each failure and a sucess handler when an attempt succeeds
.Retry(
    int maxNumberOfAttempts,
    TimeSpan retryDelay,
    Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task> exceptionFunc = null,
    Func<TMessageType, int, int, TimeSpan, Task> successFunc = null);

// Call an exceptionHandler for each failure and a sucess handler when an attempt succeeds
.Retry(
    int maxNumberOfAttempts,
    TimeSpan retryDelay,
    Func<TMessageType, Exception, int, int, TimeSpan, Task> exceptionFunc = null,
    Func<TMessageType, int, int, TimeSpan, Task> successFunc = null);

// Call an exceptionHandler for each failure and a sucess handler when an attempt succeeds
.Retry(
    int maxNumberOfAttempts,
    TimeSpan retryDelay,
    Func<TMessageType, Exception, int, int, Task> exceptionFunc = null,
    Func<TMessageType, Task> successFunc = null);

// Call an exceptionHandler for each failure and a sucess handler when an attempt succeeds
.Retry(
    int maxNumberOfAttempts,
    TimeSpan retryDelay,
    Action<TMessageType, Exception, int, int> exceptionAction,
    Action<TMessageType> successAction = null)

// Call an exceptionHandler for each failure and a sucess handler when an attempt succeeds
.Retry(
    int maxNumberOfAttempts,
    TimeSpan retryDelay,
    IMessageHandler<TMessageType> retryHandler);
```

* `numberOfAttemps` - The total number of attemps to make.
* `retryDelay` - The delay between attempts
* `exceptionFunc` / `exceptionAction` *(optional)* - The method to invoke if an exception is thrown
* `successFunc` / `successAction` *(optional)* - The method to invoke if the handler is successful
* `retryHandler` use a separate handler for retries

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .Retry(
        5,      // 5 attempts
        TimeSpan.FromSeconds(30),
        (message, exception, currentAttempt, maxNumberOfAttempts) =>
            {
                Console.WriteLine("Id:" + message.Id + " failed, attempt " + currentAttempt + "/" + maxNumberOfAttempts + ", Exception:" + exception);
            },
        message => Console.WriteLine("Message succeeded!")
        )
    .Concurrent(16)
    .Handler(async message =>
        {
            throw new Exception("FAIL!");
        });
```

#### `.Select()`
Changes the message handler chain from one message type to another type, much like LINQ's `.Select` for enumerables. 

##### Overloads
```csharp
.Select(Func<TMessageType, TNewMessageType> selector);
```

##### Examples

```csharp
public class Message
{
    public string Id { get; set; }
    public string Text { get; set; }
    public bool IsPolite { get; set; }
}

public class PoliteMessage
{
    public PoliteMessage(string id, string politeText)
    {
        this.Id = id;
        this.PoliteText = politeText;
    }

    public string Id { get; }
    public string PoliteText { get; }
}

public void SetupSubscription(IMessageBusSubscriptions<Message> bus)
{
    bus
        .Subscribe()
        .Where(message => message.IsPolite)
        .Select(message => new PoliteMessage(message.Id, message.Text)
        .Handler(message =>
            {
                Console.WriteLine("This message is guaranteed to be polite: " + message.PoliteText);
            });
}
```

As usual, you can use decorators more than once to convert between different types. 
```csharp
public class Message
{
    public string Id { get; set; }
    public string Text { get; set; }
    public bool IsPolite { get; set; }
}

public struct MessageContainer<T>
{
    public MessageContainer(T message)
    {
        this.Message = message;
    }

    public T Message { get; set; }
    public DateTime MessageDate { get; set; }
}

public void SetupSubscription(IMessageBusSubscriptions<Message> bus)
{
    bus
        .Subscribe()
        .Where(message => message.IsPolite)         // Only polite messages ;)
        .Select(message => new MessageContainer<Message>(message) // Convert to MessageContainer
        .Exception((message, exception) => Console.WriteLine("Failed delivering Message created " + message.MessageDate + ":" + exception))
        .Retry(5, TimeSpan.FromSeconds(60))
        .Concurrent(16)
        .Filter(null, messageAfterExecution => Console.WriteLine("Handled a message " + messageAfterExecution.MessageDate))    
        .Select(message => message.Message) // Convert back to Message
        .Handler(message =>
            {
                Console.WriteLine("This message is guaranteed to be polite: " + message.Text);
            });
```


#### `.SelectMany()`

##### Overloads
```csharp
```
##### Examples
```csharp
```

#### `.Semaphore()`
Limits the number of concurrent messages being handled without parallelizing.
##### Overloads
```csharp
.Semaphore(int maxNumberOfConcurrentMessages)
.Semaphore(Func<TMessageType, TKeyType> keySelector, int maxNumberOfConcurrentMessages = 1)
.Semaphore(Func<TMessageType, TKeyType> keySelector, IEqualityComparer<TKeyType> equalityComparer, int maxNumberOfConcurrentMessages = 1)
.Semaphore(Func<TMessageType, TKeyType> keySelector, KeySemaphore keySemaphore)
```
* `maxNumberOfConcurrentMessages` the number of concurrent messages to allow. 
* `keySelector` the keySelector used to define the key used to limit concurrency. 
* `keySemaphore` the shared key semaphore used to limit access from different chains. 

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .Semaphore(1)
    .Handler(async message =>
        {
            Console.WriteLine("This is guaranteed to run on a single thread at a time");
        });
```
The example above limits the concurrency to 1 simultaneous message being handled

```csharp
var subscription = bus
    .Subscribe()
    .Semaphore(msg => msg.Id, 1)
    .Handler(async message =>
        {
            Console.WriteLine("This is guaranteed to run on a single thread at a time for id " + message.Id);
        });
```
The example above limits the concurrency to 1 simultaneous message being handled with the same Id

```csharp

    var keySemaphore = new KeySemaphore<string>();

var subscription1 = bus1
    .Subscribe()
    .Semaphore(msg => msg.Id, keySemaphore)
    .Handler(async message =>
        {
            Console.WriteLine("This is guaranteed to run on a single thread at a time for id " + message.Id);
        });

var subscription2 = bus2
    .Subscribe()
    .Semaphore(msg => msg.Id, keySemaphore)
    .Handler(async message =>
        {
            Console.WriteLine("This is guaranteed to run on a single thread at a time for id " + message.Id);
        });
```
The example above limits the concurrency to 1 simultaneous message being handled with the same Id. If subscription1 is handling a message, subscription2 will have to wait if another message with the same id is published to subscription2 until subscription1 is done handling the message.

#### `.Skip()`
Skips X messages before allowing messages to pass through.

##### Overloads
```csharp
.Skip(int numberOfMessages);
```
* `numberOfMessages` the number of messages to skip before letting messages through

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .Skip(5)
    .Handler(async message =>
        {
            Console.WriteLine("The message is NOT among the first 5");
        });
```

#### `.SkipWhile()`
Skips all messages as long as the predicate returns true.

##### Overloads
```csharp
.SkipWhile(Func<TMessageType, bool> predicate);
.SkipWhile(Func<TMessageType, Task<bool>> predicate);
```
##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .SkipWhile(message => message.IsAmongTheFirst)
    .Handler(async message =>
        {
            Console.WriteLine("The message is not among the first");
        });
```

#### `.SoftFireAndForget()`
Breaks the feedback/exception chain which means that everything being synchronous coming after `.SoftFireAndForget()` is handled synchronous but it does not await asynchronous waiting.
Exceptions thrown after `.SoftFireAndForget()` does not pass `.SoftFireAndForget()`. 
Since `.SoftFireAndForget()` returns when all synchronous work is done, using `.NoDuplicates().SoftFireAndForget()` will render `.NoDuplicates()` unusable.
Using `.SoftFireAndForget()` after `.Concurrent()` will make the concurrency infinite.

`.SoftFireAndForget()` should be used early in most subscriber message chains unless you want to have the feedback and awaitability.

##### Overloads
```csharp
.SoftFireAndForget();
```

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .SoftFireAndForget()
    .Concurrent(16)
    .Handler(async message =>
        {
            Console.WriteLine("Handler");
        });
```

```csharp
var subscription = bus
    .Subscribe()
    .SoftFireAndForget()
    .Delay(TimeSpan.FromSeconds(10))
    .Handler(async message =>
        {
            Console.WriteLine("Handler");
        });
```

#### `.Take()`
Subscribes for X messages, then unsubscribes

##### Overloads
```csharp
.Take(int numberOfMessages);
```
* `numberOfMessages` the number of messages to pass through before unsubscribing

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .Take(5)
    .Handler(async message =>
        {
            Console.WriteLine("The message is among the first 5");
        });
```

#### `.TakeWhile()`
Subscribes as long as a predicate returns true, then unsubscribes

##### Overloads
```csharp
.TakeWhile(Func<TMessageType, bool> predicate);
.TakeWhile(Func<TMessageType, Task<bool>> predicate);
```
* `predicate` subscribes as long as the predicate returns true, then unsubscribes

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .TakeWhile(message => message.IsAmongTheFirst)
    .Handler(async message =>
        {
            Console.WriteLine("The message is among the first");
        });
```


#### `.WeakReference()`
`.WeakReference()` keeps the message handler as a weak reference which does not prevent it from being garbage collected. If the handler is garbage collected (reclaimed by the garbage collection) `.WeakReference()` disposes the subscription (unsubscribes).
This is usually used in MVVM applications where the framework or a DI container instantiates for example a ViewModel. When all views linked to the ViewModel are garbage collected, you want and expect the ViewModel to be garbage collected as well.
The message bus normally holds strong references to the message handlers (since it executes faster) but in this case, the ViewModels will remain active handling messages after the last View is garbage collected, which not only is a memory leak but it can lead to unexpected behavior, bugs and performance issues.

Good thing, it's easy to remedy. Use `.WeakReference()` *AS THE FINAL DECORATOR BEFORE THE HANDLER*. 
If you don't, the decorators between WeakReference and your Handler may be reclaimed by GC and thereby terminating the subscription prematurely.

`WeakReference()` disposes the subscription (unsubscribes) when a message is published and the handler is garbage collected. But if we have a condition that prevents `.WeakReference()` from getting the message. 
In the example below due to the `.Where()` decorator, if the user is deleted, it's likely never unsubscribed. To remedy this problem, `.WeakReference()` have a Garbage Collector of it's own that by default, once a minute, checks and unsubscribes all subscriptions to handlers that are reclaimed by GC.
 
##### Overloads
```csharp
.WeakReference()
.WeakReference(IWeakReferenceGarbageCollector weakReferenceGarbageCollector)
```
* `weakReferenceGarbageCollector` let's you choose a custom garbage collector.

##### Example
In the example, we pretend there is a class called BaseViewModel in your MVVM framework, containing a `Model` property of the same generic type as BaseViewModel and that it's populated with a user.
```csharp
public struct UserUpdatedEvent
{
    public User User {get; set;}
}

public class UserViewModel : BaseViewModel<User>
{
    private readonly IMessageBusSubscription userUpdatedSubscription;

    public UserViewModel(IMessageBusSubscriptions<UserUpdatedEvent> userUpdatedEvent)
    {
        // Subscribe to updates to the user
        this.userUpdatedSubscription = userUpdatedEvent
            .Subscribe()
            .Where(message => message?.User?.Id == this.Model?.Id)
            .DispatchOnCurrentContext()
            // Just before .Handler to ensure the other decorators are not reclaimed buy GC
            .WeakReference()
            .Handler(this.UserUpdatedEventHandler)
    }

    private void UserUpdatedEventHandler(UserUpdatedEvent message)
    {
        this.RefreshUI();
    }
}

```

#### `.Where()`
Filters a sequence of values based on a predicate

##### Overloads
```csharp
.Where(Func<TMessageType, bool> predicate);
.Where(Func<TMessageType, Task<bool>> predicate);
```
`predicate` - the function that is used to filter messages. Return `true` to keep the message, `false`  to drop it.

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .Where(message => message.Id == "1")
    .Handler(async message =>
        {
            Console.WriteLine("Only messages with Id 1");
        });
```

### Publishing
You have a selection of options to customize the way messages are delivered. You can customize the way messages are published to the subscribers, and you can customize the way the subscribers handle the messages.
Customizing publishing affects all messages being published to a bus, while customizing a subscription only affects that subscription.

Use custom subscriptions before custom publishing, since it it will not affect as much 

#### Customizing the bus publisher message handler chain
You can configure the bus using the same decorators you use to configure subscriptions.

Use the `.UseSubscriptionChain()` extension method on ´ConcurrentMessagesBusOptions<TMesageBus>` to decorate the dispatch message handler chain:

##### Overloads
```csharp
// Configures a chain without specifying a handler
.UseSubscriptionChain<TMessageType>(Action<MessageHandlerChainBuilder<MessageAndHandler<TMessageType>>> configureMessageHandlerChain);

// Configures a chain specifying a handler
.UseSubscriptionChain<TMessageType>(Action<MessageHandlerChainBuilder<MessageAndHandler<TMessageType>>, Func<MessageAndHandler<TMessageType>, CancellationToken, Task>> configureMessageHandlerChain);
```
* `configureMessageHandlerChain` - The method called to configure the bus options. 

The SubscriptionChain handles a message of type `MessageAndHandler<TMessageType>` for each subscription:
```csharp
public struct MessageAndHandler<TMessageType>
{
    public MessageAndHandler(TMessageType message, Func<TMessageType, CancellationToken, Task> messageHandler)
    {
        this.Message = message;
        this.MessageHandler = messageHandler;
    }

    public TMessageType Message { get; }
    public Func<TMessageType, CancellationToken, Task> MessageHandler { get; }
}
```
* `Message` - the message.
* `MessageHandler` - the message handler to execute for this message.

##### Example
```csharp
var bus = new ConcurrentMessageBus<TestMessage>(
    options => options.UseSubscriptionChain(
        chain => chain
            .Concurrent(16)
            .Filter(
                messageBeforeHandler => 
                    {
                        Console.WriteLine("Before the message is invoked");
                    },
                messageAfterHandler =>
                    {
                        Console.WriteLine("After the message was invoked")
                    });
            ));
```

Make sure you call the handler method at the end of the MHC chain or your subscribers will not be called.

## Creating your own custom MHC decorator
When you have requirements that can not be fullfilled using the existing decorators, it might be time to write your very own.

This first example is the very simple `.Where()` decorator:

```csharp
public static class WhereExtensions
{
    /// <summary>
    /// Filter messages by a predicate
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    /// <param name="messageHandlerChainBuilder">The builder</param>
    /// <param name="predicate">The predicate</param>
    /// <returns>The builder</returns>
    public static IMessageHandlerChainBuilder<TMessageType> Where<TMessageType>(
        this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
        Func<TMessageType, bool> predicate)
    {
        if (predicate == null)
        {
            return messageHandlerChainBuilder;
        }

        return messageHandlerChainBuilder.Add(innerMessageHandler => (message, cancellationToken) =>
            {
                if (predicate(message))
                {
                    return innerMessageHandler(message, cancellationToken);
                }

                return Task.CompletedTask;
            });
    }
```

To make the decorator stackable, we must return the same type as our extension parameter, 
```IMessageHandlerChainBuilder<TMessageType>```.

The `.Add()` method on `IMessagesHandlerChainBuilder` takes a single parameter - a method that is executed with the reference to the inner message handler/decorator (one step closer to the real message handler).
Consider this setup:
```csharp
    bus
        .Subscribe()
        .Where(message => message.IsPolite)         // Only polite messages ;)
        .Exception((message, exception) => Console.WriteLine("Failed delivering Message created " + message.MessageDate + ":" + exception))
        .Retry(5, TimeSpan.FromSeconds(60))
        .Handler((message, token) => {
                Console.WriteLine("The handler!");
                return Task.CompletedTask;
            })
```
When the message handler chain is built, the decorators are set up in this order:
* `Retry` is called with the message handler as parameter
* `Exception` is called with the `Retry` handler as parameter
* `Where` is called with `Exception` as parameter

You can also make the decorator a separate type. It's easier for more complicated decorators.
Here is the extension method for `.Delay()`:
```csharp
public static class DelayExtensions
{
    /// <summary>
    /// Delay handling each message by a specified time
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    /// <param name="messageHandlerChainBuilder">The builder</param>
    /// <param name="timeToWait">The timespan to await</param>
    /// <returns>The builder</returns>
    public static IMessageHandlerChainBuilder<TMessageType> Delay<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, TimeSpan timeToWait)
    {
        return messageHandlerChainBuilder.Add(currentHandler => new DelayDecorator<TMessageType>(currentHandler, timeToWait).HandleMessageAsync);
    }
}
```
And here is the decorator:
```csharp
internal class DelayDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
{
    private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

    private readonly TimeSpan timeToWait;

    public DelayDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, TimeSpan timeToWait)
    {
        this.handlerFunc = handlerFunc;
        this.timeToWait = timeToWait;
    }

    public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
    {
        await Task.Delay(this.timeToWait, token).ConfigureAwait(true);
        await this.handlerFunc(message, token).ConfigureAwait(true);
    }
}
```





## Using the message handler chain without the message bus


## Using the WireUp
A few of the decorators supports wire up, which means, you decorate your handler type with their attributes. When you `WireUp()` the handler, the decorators are applied to the subscription or publisher.
```csharp
    

```




### ASP.NET Core
Check out the ASP.NET Core example project

```csharp
using Serpent.Common.MessageBus;
using Serpent.Common.MessageBus.Extras;

public class Startup
{
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();

        // Register generic message bus singletons
        services.AddSingleton(typeof(IMessageBus<>), typeof(ConcurrentMessageBus<>));

        // These two are required if you want to be able to resolve IMessageBusPublisher<> and IMessageBusSubscriptions
        services.AddSingleton(typeof(IMessageBusPublisher<>), typeof(PublisherBridge<>));
        services.AddSingleton(typeof(IMessageBusSubscriptions<>), typeof(SubscriptionsBridge<>));

        // To resolve only based on service type
        services.AddSingleton<ReadmeService>();

        // To resolve based on message handler
        services.AddSingleton<ReadmeService, IMessageHandler<ReadmeMessage>();
    }
}

```

Resolve a service
```csharp
bus
    .Subscribe()
    .Factory(() => container.GetService<ReadmeService>());

// Or using IMessagesHandler

bus
    .Subscribe()
    .Factory(() => container.GetService<IMessageHandler<ReadmeMessage>>());
```
