# Serpent.Common.MessageBus
This is an asynchronous .NET Standard 2.0 message bus for usage in any project where a message bus is suitable.
All messages are dispatched through the .NET TPL (which is included in .NET Framework, .NET Standard and .NET Core).
Serpent.Common.MessageBus is .NET Standard 2.0, which means, you can use it on any runtime that supports .NET Standard 2.0, for example .NET Framework 4.6.1 and .NET Core 1.0. 

The message bus is implemeted `by` `ConcurrentMessageBus<TMessageType>` and has 3 interfaces:
* `IMessageBus<TMessageType>` which in turn has two interfaces:
* `IMessageBusPublisher<TMessageType>` used to publish messages to the bus
* `IMessageBusSubscriber<TMessageType>` used to subscribe to messages

## Why?
Why would I use Serpent.Common.MessageBus or any message bus in my application instead using normal method calls?
Well, I can come up with a few reasons.

* Loose coupling - Message publisher and the subscribers know nothing about each other. As long as they know about the bus and what the messages do, both subscribers and publishers can be changed, added or replaced witout affecting each other.
* Concurrency made easy - By adding only 1 line of code (`.Concurrent(16)`), you can parallelize your work on the .NET thread pool
* Reuse - Smaller components with a defined contract can more easily be reused
* Flexibility and out of the box functionality. When you have created your message handler, you can add quite some out-of-the-box functionality to it without modifying the message handler. Throttling, Exception handling, Retry, Duplicate message elimination, to name a few.
* Configurability - Adding fancy functionality like `Retry()`, with one line of code. See Message chain decorators.

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
    .Handler(handler);
```

### Unsubscribe
The Handler-function returns an IMessageBusSubscription. To unsubscribe, you can call Unsubscribe() or Dispose().

```csharp
var subscription = bus
    .Subscribe()
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
            .Factory(() => new ReadmeFactoryHandler());
    }
}
```

You can easily have your dependency injection container produce the instance of the handler. 
This is how it would look using the dependency injection in ASP NET Core.

```csharp
        bus
            .Subscribe()
            .Factory(() => container.GetService<ReadmeFactoryHandler>());
```

### The message handler chain (MHC)
An MHC (Message Handler Chain) is simply an execution tree where messages pass through. We can easily use this concept to add functionality that normally might require quite some time to write yourself.
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
2. The `.Handler()` *MHCD (MHC Decorator)* calls our specified handler.

Let's add some MHCD functionality to it!

```csharp
var subscription = bus
    .Subscribe()
    // Inserted decorators
    .NoDuplicates(message => message.Id)
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
                    if (fileText == "Warning")
                    {
                        // Do something
                    }
                }
            }
        });
```
Let's break down what happens here
1. The subscription receives the message
2. `.NoDuplicates()` stops all messages that are already being handled by the subscription
3. `.Delay()` delay the handling of all messages by 5 seconds. In this case, to make sure the system creating the files is done writing.
4. `.Concurrent()` use 16 parallel Tasks to execute incoming messages
5. `.Retry()` Makes a total of 5 attempts to execute the handler. If the handler throws an exception, it will wait 30 seconds before retrying.
6. `.LimitedThroughput()` ensures a total of 100 messages are handled per 100ms = maximum of 1000 messages/second 
7. `.Handler()` calls our specified handler.

### Message handler chain decorators (MHCD's)

* The decorators are executed in the order they are specified.
* Exceptions thrown in a message handler are propagated back in the reverse order they are specified. 
* Exceptions and feedback do not pass `.FireAndForget()`, `.SoftFireAndForget()`, `.ConcurrentFireAndForget()`, and `.LimitedThroughputFireAndForget()`.
* A decorator can be added more than once to further customize functionality. 
* Some decorators break the feedback chain which prevents exceptions and awaitability to pass through. For example if you first add `.Retry()` and then `.FireAndForget()`, Retry will not be invoked if the handler throws an exception. They are usually called `.*FireAndForget`
* The decorators can be applied both to the subscriber and publisher side.

Note! The message handler chain and it's decorators can also be used stand alone, without a bus. See examples later.

Using the decorators when configuring a subscription
```csharp

    var subscription = bus
        .Subscribe()
		.SoftFireAndForget()						// Fire and forget
		.NoDuplicates(message => message.Id)	    // Do not allow messages with an Id matching a message already being handled
		.Delay(TimeSpan.FromSeconds(5))			// Delay the message 5 seconds
		.Retry(3, TimeSpan.FromSeconds(60))		// If the two attempts in the next line fail, try again 2 more times (3 attempts)
		.Retry(2, TimeSpan.FromSeconds(5))		// If the handler fails, retry once (two attempts in total if the first fails)
        .Handler(async message =>
            {
                await this.SomeMethodAsync();
                Console.WriteLine(message.Id);
				throw new Exception("Fail!");
            });
```

#### Message handler chain decorators list
To customize the way your subscription is handled, you can add one or many subscription decorators. 

Here's a summary of the currently available decorators

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
    .Exception(message => Console.WriteLine("All 10 attempts failed"))
    .Retry(2, TimeSpan.FromSeconds(60)) // If the first 5 attempts fail, wait 60 seconds and try 5 more times
    .Exception(message => Console.WriteLine("First 5 attempts failed. Retrying in 50 seconds"))
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
Appends another message to the subscription right after the current message is handled.

##### Overloads
```csharp
.Append(Func<TMessageType, TMessageType> appendMessageFunc);
.Append(Func<TMessageType, Task<TMessageType>> appendMessageFunc);
```

##### Examples
```csharp

IMessageSubscriber<MyMessage> bus = GetSubscriber();

var subscription = bus
    .Subscribe()
    .Append(message => new MyMessage { Text = "Appended" })
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });
```

#### `.Branch()`
This is actually not a decorator, like `.BranchOut()` but instead a handler that splits the message handler chain into two ore more message handler chains.


#### `.BranchOut()`
Maybe this is not the modifier you will use first, but it can come in handy. If you''re brand new to Serpent.Common.MessageBus you might want to read about the other modifiers first.
The Branch() modifier branches the message handler to a tree. It will make your subscription act as multiple subscription branches.
Branch() will not start a new Task for the created branch or branches unless you state it specifically.
I think an example will make more sense :).

```csharp
bus
    .Subscribe()
    .NoDuplicates(message => message.Id)
    .Branch(
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
Handles up to X messages in parallel. This decorator is as much a limiting as a parallelizing. The decorator will handle X concurrent messages, queueing (FIFO) those not being handled.
For instance, you usually get a lot higher throughput, handling multiple concurrent messages when the handlers do I/O (read/write to disk, network calls etc.).
`.Concurrent()` will keep the feedback chain which will make the caller await until the message is handled, and exceptions thrown pass right back to the caller. 
This makes `.Retry()` work properly both before and after `.Concurrent()`. See examples.
If you do not need the feedback to pass through `.Concurrent()`, you can use `.ConcurrentFireAndForget()`, which is a little bit quicker.
You usually stack `.Concurrent()` with `.SoftFireAndForget()` to prevent message publishers to wait if there are a lot of messages in the queue.

##### Overloads
```csharp
.Concurrent(int maxNumberOfConcurrentMessages);
```

##### Examples
```csharp
var subscription = bus
    .Subscribe()
    // .SoftFireAndForget() is usually wise to have somewhere before .Concurrent()
    .SoftFireAndForget()
	// 16 concurrent messages being handled
    .Concurrent(16)	
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });
```

##### `.Retry()` stacked with `.Concurrent()`
```csharp
var subscription = bus
    .Subscribe()
    // .SoftFireAndForget() is usually wise to have somewhere before .Concurrent()
    .SoftFireAndForget()
    // Handle errors from last attempt
    .Exception(message => Console.WriteLine("Message id:" + message.Id + " failed all attempts"))
    // Try 5 times. First retry (out of 4) after 1 minute
    .Retry(5, TimeSpan.FromMinutes(1))
    // 16 concurrent messages being handled
    .Concurrent(16)	
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });
```
The reason we have `.Retry()` before `.Concurrent()`, is to prevent locking one of the concurrent message handler slots when a message handler fails. 

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
	// 16 concurrent messages being handled
	.ConcurrentFireAndForget(16)	
    .Handler(async message =>
        {
            await this.SomeMethodAsync();
            Console.WriteLine(message.Id);
        });
```

#### `.Delay()`
Delay the message chain a specified time.

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
    .Delay(TimeSpan.FromSeconds(10)) // delay 10 seconds before handling the message	
    .Handler(async message =>
        {
            await this.SomeMethodAsyncInvoked10SecondsLaterAsync();
            Console.WriteLine(message.Id);
        });
```


```csharp
var subscription = bus
    .Subscribe()
    .SoftFireAndForget()    // Don't have the publisher await the delay
    .Delay(TimeSpan.FromSeconds(10)) // delay 10 seconds before handling the message
    .Handler(async message =>
        {
            await this.SomeMethodAsyncInvoked10SecondsLaterAsync();
            Console.WriteLine(message.Id);
        });

```

#### `.Distinct()`


#### `.Exception()`
`Exception()` invokes a method if the message handler (or a MHC Decorator below in the chain) throws an exception. If the exception handler returns nothing or false, the exception is caught and it does not propagate further up the chain.
If you want the exception to continue it's journey up the chain, for exampel to use the `Retry()` decorator, return true.

This decorator for example can be useful for logging exceptions or trigger something else to happen as a response. 

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
	.Exception((message, exception) => {
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

Before and after
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
NOTE! `.FireAndForget` should be avoided if possible. 
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
A new period starts as soon as the last period ends. Messages not allowed to pass through are queued. The queue is FIFO (first in, first out).
Messages are not evenly throttled within the period, which means, if you use `.LimitedThroughput(1000, TimeSpan.FromSeconds(1))` all 1000 messages allowed to pass through can come the periods first millisecond.
To throttle messages more evenly, you can make the period smaller - `.LimitedThroughput(100, TimeSpan.FromSeconds(0.1))`. 

`.LimitedThroughput()` count messages at the arrival time, which means only X message handlers can be started per period.

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
            Console.WriteLine("Throttled!");
        });

var subscription = bus
    .Subscribe()
    .LimitedThroughput(200, TimeSpan.FromMilliseconds(1000))
    .Handler(message =>
        {
            Console.WriteLine("Throttled!");
        });
```
#### `.LimitedThroughputFireAndForget()`
This is roughly the same functionality as `SoftFireAndForget().LimitedThroughput()` but since `.LimitedThroughputFireAndForget()` does not keep the feedback chain, it's has a little less overhead.

##### Overloads
```csharp
.LimitedThroughputFireAndForget(int maxMessagesPerPeriod, TimeSpan? periodSpan = null)
```

#### `.NoDuplicates()`
Make all messages waiting to be handled to be unique, based on a key.
This means if a message enters `.NoDuplicates()` with the same key as another message that has already entered but not exited the same `.NoDuplicates()`, it is dropped.

If you for instance have a message handler that is responsible for deleting a file from disk, the file can only be deleted once, and therefore having duplicate messages in the chain, based on full path and filename is unnecessary.
Another example would be a handler responsible for loading a project from a database. As long as the data is not fully loaded, it would be better to just wait for the first load request to finish.

##### Overloads
```csharp
.NoDuplicates(Func<TMessageType, TKeyType> keySelector);
.NoDuplicates(Func<TMessageType, TKeyType> keySelector, IEqualityComparer<TKeyType> equalityComparer);
```

##### Examples


#### `.Retry()`

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

As usual, you can use the decorators more than once to convert between different types. In the following example we start by 
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
}
```

#### `.Semaphore()`

#### `.SoftFireAndForget()`

#### `.TaskScheduler()`

#### `.Where()`

### Publishing
You have a selection of options to customize the way messages are delivered. You can customize the way messages are published to the subscribers, and you can customize the way the subscribers handle the messages.
Customizing publishing affects all messages being published to a bus, while customizing a subscription only affects that subscription.

Use custom subscriptions before custom publishing, since it''s it will not change as much 

### Publishing
The default method of publishing messages to the subscribers is Parallel. The subscriber methods are invoked in parallel and are then awaited as a group. See ParallelPublisher below.

Publishing is implemented by Publishers, types deriving from BusPublisher<TMessageType>. Serpent.Common.MessageBus has implementation for a lot of common scenarios.

To change publisher, change the BusPublisher property on the ConcurrentMessageBusOptions<TMessageType>, either directly or by using one of the extension methods.

3 examples of how to use FireAndForgetPublisher:

```csharp

	var bus = new ConcurrentMessageBus<int>(options => {
		options.BusPublisher = new FireAndForgetPublisher<int>();
	});

	var bus = new ConcurrentMessageBus<int>(options => {
		options.BusPublisher = FireAndForgetPublisher<int>.Default;
	});

	var bus = new ConcurrentMessageBus<int>(options => {
		options.UseFireAndForgetPublisher();
	});
```

You can also implement your own bus publishers by deriving from BusPublisher<TMessageType> and by implementing/overriding PublishAsync(IEnumerable<ISubscription<T>> subscriptions, T message).

##### Bus Publishers
The bus publishers are categorized in Publishers and Decorators. Some of the bus publishers are dectorators with a default Publisher.
The publishers will actually call the subscribers. The decorators will modify the behaviour of a Publisher.

##### BackgroundSemaphorePublisher (Decorator)
Set the concurrency level and then messages handled will be invoked on one of the tasks, just awaiting to be activated and publish the message.
This publisher is a Fire And Forget publisher since all messages are queued when PublishAsync is called and handled as soon as the concurrency allows it.

The concurrency level decides the number of messages being processed in parallel, not the number of handlers processing a message in parallel.

The difference between SemaphorePublisher and BackgroundSemaphorePublisher is that for BackgroundSemaphorePublisher, PublishAsync queues the message and the current subscribers and then returns. One of the worker tasks will publish the message as soon as possible.
The mechanisms behind them are different. Use BackgroundSemaphorePublisher to THROTTLE the number of concurrent messages being handled. 

The publisher use ParallelPublisher<T> by default for publishing messages but this can be customized through the constructor.

BackgroundSemaphorePublisher is similar to using a FireAndForgetPublisher decorating a SemaphorePublisher but with different mechanisms behind it.

##### FireAndForgetPublisher (Dectorator)
Makes PublishAsync return without allowing the caller to decide if all handlers have been called.

The publisher use ParallelPublisher<T> by default for publishing messages but this can be customized through the constructor.

```csharp

	// Instantiate a new FireAndForgetPublisher
	var bus = new ConcurrentMessageBus<int>(options => {
		options.BusPublisher = new FireAndForgetPublisher<int>();
	});

	// Using the default FireAndForgetPublisher
	var bus = new ConcurrentMessageBus<int>(options => {
		options.BusPublisher = FireAndForgetPublisher<int>.Default;
	});

	// Using the options extension method
	var bus = new ConcurrentMessageBus<int>(options => {
		options.UseFireAndForgetPublisher();
	});

            // Using the options extension method to decorate ParallelPublisher with FireAndForgetPublisher
            var bus = new ConcurrentMessageBus<int>(options =>
            {
                options.UseFireAndForgetPublisher(ParallelPublisher<int>.Default);
            });

```

##### ForcedParallelPublisher
A message is sent to all handler functions in parallel, not awaiting each handler to finish before the next handler is invoked
The Task returned by PublishAsync is done when all handlers Tasks are done

##### FuncPublisher
The subscribers and message are sent to a provided function. 

Examples:
```csharp

	// Instantiate a new FuncPublisher to drop all messages
	var bus = new ConcurrentMessageBus<int>(options => {
		options.BusPublisher = new FuncPublisher<int>((subscribers, message) => Task.CompletedTask);
	});

	// Using the options extension method
	var bus = new ConcurrentMessageBus<int>(options => {
		options.UseFuncPublisher((subscribers, message) => Task.CompletedTask);
	});


	// Using the options extension method to log all messages published and then send them to the default ParallelPublisher
	// The logger of your choice
	ILogger log = GetLogger();

	// Make a reference to prevent calling unnecessary calls to the ParallelPublisher<int>.Default property.
    Func<IEnumerable<ISubscription<int>>, int, Task> publishAsync = ParallelPublisher<int>.Default.PublishAsync;

    var bus = new ConcurrentMessageBus<int>(
        options =>
            {
                options.UseFuncPublisher((subscribers, message) =>
                    {
                        log.LogTrace(message + " was published");
                        return publishAsync(subscribers, message);
                    });
            });
	});
```

##### LoggingPublisher

##### ParallelPublisher
A message is sent to all handler functions serially, not awaiting each handler to finish before the next handler is invoked
This is the DEFAULT value. If all handlers are written to follow the TPL guidelines (never blocking), this is usually the best option. 
The Task returned by PublishAsync is done when all handlers Tasks are done.

##### SemaphorePublisher (Decorator)
This concurrency level decides the number messages the bus can publish simultaneously. 

The difference between SemaphorePublisher and BackgroundSemaphorePublisher is that for SemaphorePublisher, PublishAsync does not return before all subscribers have been called.
The mechanisms behind them are different. Use SemaphorePublisher to LIMIT the number of concurrent messages being handled.

The publisher use ParallelPublisher<T> by default for publishing messages but this can be customized through the constructor.

##### SerialPublisher
A message is sent to the handler functions serially, which means the first subscriber must finish handling the message before the second handler is invoked.
The Task returned by PublishAsync is done when all handler functions Tasks are done

##### SingleReceiverPublisher
A message is sent to only a single subscriber, rotating so every subscriber get approximtely the same number of messages.
For a bus where subscribers are added and removed frequently, messages may be sent in a higher frequency to long time subscribers.


#### Implement your own BusPublisher
Inhert from BusPublisher<T>, use PublishTypeType.Custom and set CustomBusPublisher to your new type.



### Strong and weak references



## Advanced topics




### Subscription handler factories
Instead of having a single(ton) instance handling all messages you can setup a subscription handler factory that instantates a new handler for each message being published to the bus



### Semaphore handler by key