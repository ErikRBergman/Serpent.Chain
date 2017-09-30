# Serpent.Common.MessageBus
This is an asynchronous .NET Standard 2.0 message bus for usage in any project where a message bus is suitable.
All messages are dispatched through the .NET TPL (which is included in .NET Framework, .NET Standard and .NET Core).
Serpent.Common.MessageBus is .NET Standard 2.0, which means, you can use it on any runtime that supports .NET Standard 2.0, for example .NET Framework 4.6.1 and .NET Core 1.0. 

The message bus is implemeted `by` `ConcurrentMessageBus<TMessageType>` and has 3 interfaces:
* `IMessageBus<TMessageType>` which in turn has two interfaces:
* `IMessageBusPublisher<TMessageType>` used to publish messages to the bus
* `IMessageBusSubscriber<TMessageType>` used to subscribe to messages

Feel free to fork the project, make changes and send pull requests, report errors, suggestions, ideas, ask questions etc.

## Why?
Why would I use Serpent.Common.MessageBus or any message bus in my application instead using normal method calls?
Well, I can come up with a few reasons.

* Loose coupling - Message publisher and the subscribers know nothing about each other. As long as they know about the bus and what the messages do, both subscribers and publishers can be changed, added or replaced witout affecting each other.
* Concurrency made easy - By adding only 1 line of code (`.Concurrent(16)`), you can parallelize your work on the .NET thread pool
* Reuse - Smaller components with a defined contract can more easily be reused
* Flexibility and out of the box functionality. When you have created your message handler, you can add quite some out-of-the-box functionality to it without modifying the message handler. Throttling, Exception handling, Retry, Duplicate message elimination, to name a few.
* Configurability - Adding fancy functionality like `Retry()`, with one line of code. See Message chain decorators.

## How to install
If you use Visual Studio, open the NuGet client for your project and find `Serpent.Common.MessageBus`.

or

Open the `Package Manager Console` and type:

`install-package Serpent.Common.MessageBus`

## Example

```csharp
internal class ExampleMessage
{
    public string Id { get; set; }
}

// Create a message bus
var bus = new ConcurrentMessageBus<ExampleMessage>();

// Add a synchronous subscriber
var subscription = bus
        .Subscribe()
        // .SoftFireAndForget() // De-couple the publisher from the subscriber execution chain
        // .Concurrent(16) // Up to 16 concurrent tasks will handle messges
        // .Retry(3, TimeSpan.FromSeconds(30)) // Try up to 3 times with 30 sec. delay between
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
```

## Subscribing to messages
When you subscribe to a bus, you need to specify a message handler. You can have an inline function handling messages, a function of your choice, a type that implements `IMessageHandler<TMessageType>` or a factory instantiating a type that implements `IMessageHandler<TMessageType>`.

#### Subscribe and handle synchronously
```csharp
var subscription = bus
    .Subscribe()
    .Handler(message =>
        {
            Console.WriteLine(message.Id);
        });
```
Note! The message bus works internally fully with TPL, but if you do not have a need for any async operations, like I/O, you can use one of the handler extensions to make your code more readable.

#### Subscribe and handle asynchronously
```csharp
var subscription = bus
    .Subscribe()
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });
```

`.Handler()` returns a subscription that you call to unsubscribe.

#### `.Subscribe().SoftFireAndForget()`
Most often, you do not want the bus to track execution of your message handlers. The most common and best way to do this is by using `.SoftFireAndForget()`.
```csharp
var subscription = bus
    .Subscribe()
    .SoftFireAndForget()
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });
```

The `.Handler()` method returns a subscription that you can call to unsubscribe.

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
    .SoftFireAndForget()
    .Handler(handler);
```

### Unsubscribe
The Handler-function returns an IMessageBusSubscription. To unsubscribe, you can call Unsubscribe() or Dispose().

```csharp
var subscription = bus
    .Subscribe()
    .SoftFireAndForget()
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });

...

subscription.Unsubscribe();
// or
subscription.Dispose();
```

You can also use a SubscriptionWrapper that unsubscribes when it goes out of scope.
```csharp
public class HandlerClass
{
    private readonly SubscriptionWrapper wrapper;

    HandlerClass(IMessageSubscriber<ExampleMessage> bus)
    {
        this.wrapper = bus
            .Subscribe()
            .SoftFireAndForget()
            .Handler(async message =>
                {
                    await this.SomeMethodAsync();
                    Console.WriteLine(message.Id);
                })
            .Wrapper();
	}
}
```

### Using a `.Factory()` to instantiate a handler for each message
Note that this handler implements IDisposable, which is not a requirement. When using a factory to instantiate an IDisposable type, the type is automatically disposed when the message has been handled.
This approach can come in handy and simplify your code if, for example, your handler class use resources that can can only be used for a short period of time.

```csharp

internal class ReadmeFactoryHandler : IMessageHandler<ExampleMessage>, IDisposable
{
    public void Dispose()
    {
        // And if the type implements IDisposable, the Dispose method is called secondly
    }

    public async Task HandleMessageAsync(ExampleMessage message)
    {
        // The HandleMessageAsync method is called first
    }
}

internal class ReadmeFactoryHandlerSetup
{
    public void SetupSubscription(IMessageBusSubscriber<ExampleMessage> bus)
    {
        bus
            .Subscribe()
            .SoftFireAndForget()
            .Factory(() => new ReadmeFactoryHandler());
    }
}
```

You can easily have your dependency injection container produce the instance of the handler. 
This is how it would look using the dependency injection in ASP NET Core.

```csharp
        bus
            .Subscribe()
            .SoftFireAndForget()
            .Factory(() => container.GetService<ReadmeFactoryHandler>());
```

### The message handler chain (MHC)
MHC (Message Handler Chain) is the execution tree where messages pass through. We can easily use this concept to add functionality that normally might require quite some time to write yourself.
The MHC exists both on the subscriber and publisher side.

When a subscription receives a message, it passes through the MHC before it reaches the `.Handler()` or `.Factory()` *MHCD (MHC Decorator)* which is the last step of the message handler chain.
Example:

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
    .SoftFireAndForget()
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
2. `.SoftFireAndForget()` break the feedback chain so publishers won't wait for the message handling
3. `.NoDuplicates()` stops/drops all messages that are already being handled by the subscription, making sure the same file is not read more than once concurrently 
4. `.Delay()` delay the handling of all messages by 5 seconds. In this case, to make sure the "other" system creating the files is done writing before we start reading.
5. `.Retry()` make a total of 5 attempts to read the file. If the handler throws an exception, `.Retry()` will wait 30 seconds before trying again
6. `.Concurrent()` processes up to 16 files concurrently, queueing any excess
7. `.LimitedThroughput()` limits throughput to maximum of 100 messages per 100ms = 1000 messages/second 
8. `.Handler()` calls our code reading the file

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
            .SoftFireAndForget()                    // Fire and forget
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

#### Message handler chain decorators list
To customize the way your subscription is handled, you can add one or many subscription decorators. 

Here's a summary of the currently available decorators. If you have requirements that these decorators does not support, you can write your own decorators (see the chapter Custom MHC Decorators). 

* `.Append()` - Append a message for each message. Like LINQ `.Append()`.
* `.Branch()` - Split the MHC into two or more parallel trees. 
* `.BranchOut()` - Branch the MHC tree into one or more MHC trees parallel to the normal MHC tree.
* `.Concurrent()` - Parallelize and handle X concurrent messages.
* `.ConcurrentFireAndForget()` - Parallelize and handle X concurrent messages but does not provide delivery feedback and does not pass through exceptions.
* `.Delay()` - Delay the execution of the message handler.
* `.Distinct()` - Only pass through unique messages based on key.
* `.Exception()` - Handle exceptions not handled by the message handler.
* `.Filter()` - Execute a method before and after the execution of the message handler. Can also filter messages to stop the message from being processed further.
* `.FireAndForget()` - Spawn a new Task to execute each message.
* `.First()` - Only let 1 message pass through. Optionally based by a predicate (which is the same as `.Where().First()`).
* `.LimitedThroughput()` - Limit the throughput to X messages per period. For example, 100 messages per second. Or 10 messages per 100 ms.
* `.LimitedThroughputFireAndForget()` - Same as `.LimitedThroughput()` but break the feedback chain.
* `.NoDuplicates()` - Drop all duplicate messages by key. Duplicate messages are dropped.
* `.Prepend()` - Prepend a message for each message handled. Like LINQ `.Prepend()`.
* `.Retry()` - Retry after TimeSpan, X times to deliver a message if the message handler fails (throws an exception)
* `.Select()` - Change message message type for the remaining message handler chain. Like LINQ `.Select()`.
* `.SelectMany()` - Change message message type for the remaining message handler chain and extract messages from an enumerable. Like LINQ `.SelectMany()`.
* `.Semaphore()` - Limit the number of concurrent messages being handled by this subscription.
* `.SoftFireAndForget()` - Executes the synchronous parts of the next MHCD or Handler, synchronous but everything asynchronous is executed without feedback. 
* `.Skip()` - Skip the first X messages. Like LINQ `.Skip()`.
* `.SkipWhile()` - Skip messages as long as the predicate succeeds. Like LINQ `.SkipWhile()`.
* `.Take()` - Only let X messages pass through. Like LINQ `.Take()`.
* `.TakeWhile()` - Only let messages pass through as long as a predicate succeeds. The same as `.Where().Take()`. Like LINQ `.TakeWhere()`.
* `.TaskScheduler()` - Have the messages despatched to a new Task on a specified Task Scheduler. For example, to have all messages handled by the UI thread.
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

IMessageSubscriber<MyMessage> bus = new ConcurrentMessageBus<MyMessage>();

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

IMessageSubscriber<MyMessage> bus = new ConcurrentMessageBus<MyMessage>();

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

IMessageSubscriber<MyMessage> bus = new ConcurrentMessageBus<MyMessage>();

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

IMessageSubscriber<MyMessage> bus = new ConcurrentMessageBus<MyMessage>();

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
The feedback chain is intact through Branch, so if one of the branches throws an exception which is not handled, it is passed up the chain and can potentially prevent other branches to complete. If there is no FireAndForget, the publisher can await the delivery of the message to all branches.

##### Overloads
```csharp
.Branch(params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches);
```
* `branches` - the action that returns an array of MHC branches. See the examples.
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
            .SoftFireAndForget()
            .Delay(TimeSpan.FromSeconds(10))
            .Filter(message => message.Id == "Message 2")
            .Handler(message => Console.WriteLine("I only handle Message 2")))
    .Handler(message => Console.WriteLine("I handle all messages"));
```
The subscription will not handle any concurrent duplicates and is branched to two separate message handlers.
Message 2 will be handled by the branched message handler 10 seconds after the subscription receives it while the normal handler will handle all messages immediately.
Since we don''t want the Publisher to await the delayed delivery, we stack `.SoftFireAndForget()` and `.Delay()`.

#### `.Concurrent()`
Parallelizes the message handler chain to a maximum of X concurrent messages. Messages that arrive to `.Concurrent()` are queued and handled as soon as possible, in a FIFO (first in first out) manner. 
`.Concurrent()` both parallelizes and limits parallelism to your defined level. If you use `.Concurrent(20)` to send e-mails, up to 20 and no more than 20 e-mails are sent concurrently.

You usually get a lot higher throughput by handling multiple concurrent messages, especially when the handlers perform I/O (read/write to disk, sending e-mails, calling web services, web apis, sending data to an Azure service bus, etc.).
Often there is also a threshold where too much parallelism degrades performance. Sending 20 e-mails concurrently is likely much faster than 1, but sending 100 e-mails concurrently may put too much pressure on the SMTP server and can be slower than 20.
It's often wise to make the level of parallelism configurable. You may experience great performance at some level on your developer machine, and a different one when you deploy to your test and production environments.

`.Concurrent()` will keep the feedback chain which will allow the caller to await until the message is handled. Exceptions thrown pass right back to the caller. 
This makes decorators like `.Retry()` work properly both before and after `.Concurrent()`. See the examples.

If you do not need the feedback chain, you can use `.ConcurrentFireAndForget()`, which has slightly smaller execution footprint.

##### Overloads
```csharp
.Concurrent(int maxNumberOfConcurrentMessages);
```

##### Examples
Let's say you want to send a newsletter to a 5000 of recipients. If you don't parallelize the process, it may take quite a while. 

```csharp
var smtpClient = new SmtpClient();

var subscription = bus
    .Subscribe()
    .SoftFireAndForget()
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
We specify `.Retry()` before `.Concurrent()` to process other messages if an exception is thrown. If we add `.Retry()` after `.Concurrent()` a message failing will use one of the 20 concurrent handlers for upp to 4 minutes.

```csharp
var smtpClient = new SmtpClient();

var subscription = bus
    .Subscribe()
    // .SoftFireAndForget() is usually wise to have somewhere before .Concurrent()
    .SoftFireAndForget()
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
The reason we have `.Retry()` before `.Concurrent()`, is to prevent locking one of the 16 message handler slots while waiting to retry.

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
Delay the message handler chain a specified time.

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
    .SoftFireAndForget()
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
    .SoftFireAndForget()
    .Delay(10000)) // delay 100000 milliseconds before handling the message
    .Handler(async message =>
        {
            await this.SomeMethodAsyncInvoked10SecondsLaterAsync();
            Console.WriteLine(message.Id);
        });
```

#### `.Distinct()`
Only allow a message with a certain key to be handled once.
##### Overloads
```csharp
.Distinct(Func<TMessageType, TKeyType> keySelector);
.Distinct(Func<TMessageType, TKeyType> keySelector, IEqualityComparer<TKeyType> equalityComparer);
```

##### Example
```csharp
var subscription = bus
    .Subscribe()
    .Distinct(message => message.Id)    // Only allow a message with a certain key to be delivered once
    .SoftFireAndForget()
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
.Exception<TMessageType>(Func<TMessageType, Exception, Task<bool>> exceptionHandlerFunc);
.Exception<TMessageType>(Func<TMessageType, Exception, Task> exceptionHandlerFunc);
.Exception<TMessageType>(Func<TMessageType, Exception, bool> exceptionHandlerFunc);
.Exception<TMessageType>(Action<TMessageType, Exception> exceptionHandlerAction);
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
.Filter(Func<TMessageType, Task<bool>> beforeInvoke = null, Func<TMessageType, Task> afterInvoke = null);
.Filter(Func<TMessageType, bool> beforeInvoke = null, Action<TMessageType> afterInvoke = null);
.Filter(Action<TMessageType> beforeInvoke = null, Action<TMessageType> afterInvoke = null);
.Filter(Func<TMessageType, Func<TMessageType, Task>, Task> filterFunc);
```
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
            return false; // Stop the message here
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
    .SoftFireAndForget()
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
    .SoftFireAndForget()
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
Allows you to configure the message handler chain to retry if the message handler fails.
You specify the number of attempts (not retries).

##### Overloads
```csharp
.Retry<TMessageType>(
            int maxNumberOfAttempts,
            TimeSpan retryDelay,
            Func<TMessageType, Exception, int, int, Task> exceptionFunc = null,
            Func<TMessageType, Task> successFunc = null);

.Retry<TMessageType>(
            int maxNumberOfAttempts,
            TimeSpan retryDelay,
            Action<TMessageType, Exception, int, int> exceptionAction,
            Action<TMessageType> successAction = null)
```
* `numberOfAttemps` - The total number of attemps to make.
* `retryDelay` - The delay between attempts
* `exceptionFunc` / `exceptionAction` *(optional)* - The method to invoke if an exception is thrown
* `successFunc` / `successAction` *(optional)* - The method to invoke if the handler is successful

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    .SoftFireAndForget()
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

public void SetupSubscription(IMessageBusSubscriber<Message> bus)
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

public void SetupSubscription(IMessageBusSubscriber<Message> bus)
{
    bus
        .Subscribe()
        .Where(message => message.IsPolite)         // Only polite messages ;)
        .Select(message => new MessageContainer<Message>(message) // Convert to MessageContainer
        .SoftFireAndForget()
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



#### `.Skip()`

##### Overloads
```csharp
```
##### Examples
```csharp
```




#### `.SkipWhile()`

##### Overloads
```csharp
```
##### Examples
```csharp
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
    .SoftFireAndForget() // wise to have before .Concurrent
    .Concurrent(16)
    .Handler(async message =>
        {
            Console.WriteLine("Handler");
        });
```

```csharp
var subscription = bus
    .Subscribe()
    .SoftFireAndForget() // wise to have before .Concurrent
    .Delay(TimeSpan.FromSeconds(10))
    .Handler(async message =>
        {
            Console.WriteLine("Handler");
        });
```

#### `.Take()`

##### Overloads
```csharp
```
##### Examples
```csharp
```


#### `.TakeWhile()`

##### Overloads
```csharp
```
##### Examples
```csharp
```

#### `.TaskScheduler()`
Invoke all calls on a specified Task Scheduler. This can come in handy if you use the message bus to handle messages on a UI where it's important that the messages are invoked on the UI thread.

##### Overloads
```csharp
```
##### Examples
```csharp
```

#### `.Where()`
Only forwards messages for which the `predicate` returns true.
##### Overloads
```csharp
.Where(Func<TMessageType, bool> predicate);
.Where(Func<TMessageType, Task<bool>> predicate);
```
`predicate` - the function that is used to filter messages.

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

Use `.Dispatch()` extension method on ´ConcurrentMessagesBusOptions<TMesageBus>` to change the dispatch message handler chain:

##### Overloads
```csharp
.Dispatch<TMessageType>(Action<MessageHandlerChainBuilder<MessageAndSubscription<TMessageType>>, Func<MessageAndSubscription<TMessageType>, Task>> setupMessageHandlerChainAction);
```

The message in the dispatch message handler chain is of type `MessageAndSubscription<TMessageType>`:
```csharp
public struct MessageAndSubscription<TMessageType>
{
    public MessageAndSubscription(TMessageType message, ISubscription<TMessageType> subscription)
    {
        this.Message = message;
        this.Subscription = subscription;
    }

    public TMessageType Message { get; }

    public ISubscription<TMessageType> Subscription { get; }
}
```
* `Message` is the message being dispatched.
* `Subscription` is the subscription message handler chain.

Make sure you call the handler method at the end of the MHC chain or your subscribers will not be called.

##### Example
```csharp
var bus = new ConcurrentMessageBus<TestMessage>(
    options => options.Dispatch(
    // chain - the dispatch message handler chain
    // handler - the handler 
        (chain, handler) =>
            {
                chain
                    .SoftFireAndForget()
                    .Concurrent(16)
                    .Filter(
                        message =>
                            {
                                Console.WriteLine("Before the message is invoked");
                            },
                        message =>
                            {
                                Console.WriteLine("After the message was invoked")
                            })
                    .Handler(handler);
            }));
```

### Strong and weak references
Strong references to your subscriptions will keep your subscribers from being garbage collected. 
In situations where you control the lifetime of your subscribers you can unsubscribe when your object no longer needs to handle messages. 
In situations where you do not control the creation and disposition of the subscribers, like some MVVM frameworks, it can be beneficial to use weak references which does not prevent the subscribers from being garbage collected. When the framework's last reference to your subscriber object is removed, .NET framework will GC the object and prevent it from handling messages.

## Advanced topics
