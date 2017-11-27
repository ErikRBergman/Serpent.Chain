# Serpent.Chain

|Branch|.NET Core release build|Better code|
|:--:|:--:|:--:|
|master|[![Build Status](https://travis-ci.org/ErikRBergman/Serpent.Chain.svg?branch=master)](https://travis-ci.org/ErikRBergman/Serpent.MessageBus)|[![BCH compliance](https://bettercodehub.com/edge/badge/ErikRBergman/Serpent.Chain?branch=master)](https://bettercodehub.com/)|


## Introduction
Serpent chain started as a part of Serpent.MessageBus as a way to customize the behavior of the subscriptions. 
Discussing it with various people in the field, I found out, a few wanted the behaviour of many of the decorators without having a message bus. 

Serpent chain is a way to add various common cross cutting concerns (decorators) to a service. Serpent.Chain contains a set of decorators that you use rather often.

For example, you've created an API that reads projects from your database. If 

WORK IN PROGRESS

.....


Feel free to fork the project, make changes and send pull requests, report errors, suggestions, ideas, ask questions etc.

## How to install
If you use Visual Studio, open the NuGet client for your project and find `Serpent.Chain`.

or

Open the `Package Manager Console` and type:

`install-package Serpent.Chain`

To start using the chain, add
```csharp
using Serpent.Chain;
```

## Example
```csharp

```

## Creating chains



### Decorators
Decorators are functionallity you add to a chain that change the way a chain behaves without changing the code docing the actual task at hand.

* Decorators can be stacked
* The decorators are executed in the order they are specified.
* Exceptions thrown in a message handler are propagated back in the reverse order they are specified. 
* Exceptions and awaitability do not pass `.FireAndForget()`, `.SoftFireAndForget()`, `.ConcurrentFireAndForget()`, and `.LimitedThroughputFireAndForget()`.
* A decorator can be added more than once to the message chain to further customize functionality. 
* The decorators can be applied both to your subscriptions and directly to the bus.

Note! The message handler chain and it's decorators can also be used stand alone, without a bus. See examples later.

Using the decorators when configuring a subscription
```csharp

    var func = Create.Func<Message>(b => b
        .NoDuplicates(message => message.Id)    // Do not allow messages with an Id matching a message already being handled
        .Delay(TimeSpan.FromSeconds(5))         // Delay the message 5 seconds
        .Retry(3, TimeSpan.FromSeconds(60))     // If the two attempts in the next line fail, try again 2 more times (3 attempts)
        .Retry(2, TimeSpan.FromSeconds(5))      // If the handler fails, retry once (two attempts in total if the first fails)
        .Handler(async message =>
        		{
            		await this.SomeMethodAsync();
                Console.WriteLine(message.Id);
                throw new Exception("Fail!");
			}));
```
#### Decorators reference
To customize the way your chain works, you can add one or more decorators. 

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

var func = Create.Func<Message>(b => b
    .SoftFireAndForget()                    // Don't let the publisher await no more
    .NoDuplicates(message => message.Id)    // Drop all messages already in the message chain based on key
    .Where(message => message.IsPolite)     // Only allow plite messages to pass through
    .Exception(message => Console.WriteLine("All 10 attempts failed"))  // no exceptions will pass this point
    .Retry(2, TimeSpan.FromSeconds(60)) // If the first 5 attempts fail, wait 60 seconds and try 5 more times
    .Exception(message => { Console.WriteLine("First 5 attempts failed. Retrying in 50 seconds"; return true; })) // true is to have the exception to continue up the chain
    .Retry(5, TimeSpan.FromSeconds(10)) // Try 5 times, with a 10 second delay between failures
    .Concurrent(16) // 16 concurrent handlers
    .Handler(message => Console.WriteLine("I handle all messages")));
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
You specify the number of attempts (not retries). If all attempts fail, `Retry()` throws a `Serpent.MessageBus.Exceptions.RetryFailedException` that contains the number of attempts, retry delay and a collection of all exceptions that was thrown.

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

Use the `.UseSubscriptionChain()` extension method on �ConcurrentMessagesBusOptions<TMesageBus>` to decorate the dispatch message handler chain:

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
using Serpent.MessageBus;
using Serpent.MessageBus.Extras;

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
