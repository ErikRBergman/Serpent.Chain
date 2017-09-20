# Serpent.Common.MessageBus
This is an asynchronous .NET Standard 2.0 message bus for usage in any project where a message bus is suitable.
All messages are dispatched through the .NET TPL (which is included in .NET Framework

## Example

```csharp
    internal class ExampleMessage
    {
        public string Id { get; set; }
    }

	...

    // Create a message bus
    var bus = new ConcurrentMessageBus<ExampleMessage>();

    // Add a synchronous subscriber
    var subscriber = bus
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


  // TODO: Add example
```




## Customizations

### Publishing and handling messages
You have a selection of options to customize the way messages are handled. You can customize the way messages are published to the subscribers, and you can customize the way the subscribers handle the messages.
Customizing publishing affects all messages being published, while customizing the subscriptions affects only that subscription.

My recommendation would be to use custom subscriptions before custom publishing, since it''s it will not change as much 

### Subscriptions
All subscriptions except the normal subscription can be stacked/decorated to combine their functionallity.

#### Normal subscription
Just call the Subscribe method.

Example:
```csharp
  // TODO: Add example
```

#### FireAndForgetSubscription
Invokes all handlers in a Fire and Forget manner.

Example:
```csharp
  var subscriptionWrapper = messageBusSubscriber.CreateFireAndForgetSubscription(async message => { Console.WriteLine("Fired and forgotten"); })
```

#### BackgroundSemaphoreSubscription
Allows you to set the number of concurrent handler method dispatches. 
The subscription will enqueue all incoming messages in a fire and forget manner and as soon as any of the concurrent workers (TPL Tasks) are available, the handler method is invoked.

The subscription will not use any system resources while inactive, since the workers are awaiting a SemaphoreSlim to be released.

Example:
```csharp
	var concurrencyLevel = 2;
	var subscriptionWrapper = messageBusSubscriber.CreateBackgroundSemaphoreSubscription(async message => { await Task.Delay(2000); Console.WriteLine("Called concurrently"); }, concurrencyLevel);
	
	for (var i = 0; i < 100; i++)
	{
		messageBusPublisher.PublishAsync(new SampleMessage());
	}

```
In this example, 2 messages are handled simulataneously and since the handler takes 2 seconds to execute, up to 2 messages are dispatched every 2 seconds.

#### BackgroundSemaphoreDuplicateEliminatingSubscription
This is a variation of the BackgroundSemaphoreSubscription that will not add a duplicate message to the queue, based on a key selector method.

A new message with the same key is allowed to be added again AFTER the handler is invoked. If you want to allow adding messages to the queue while a message is being handled, i suggest you couple it with a FireAndForgetSubscription. See the examples

Eliminate duplicates by KEY
```csharp
	var concurrencyLevel = 2;
	var subscriptionWrapper = messageBusSubscriber.CreateBackgroundSemaphoreWithDuplicateEliminationSubscription(async message => { await Task.Delay(2000); Console.WriteLine("Called concurrently"); }, message => message.Id, concurrencyLevel);

	for (var i = 0; i < 100; i++)
	{
		messageBusPublisher.PublishAsync(new SampleMessage(i % 10));
	}
```
In this example, the duplicate elimination key is the id field. If you have advanced keys, like structs, make sure you have implemented Equals() and GetHashCode() on them for performance.
Only 10 messages will be added to the queue and dispatched to the handler unless one of the messages are handled before the loop adding them is done


Eliminate duplicates by INSTANCE
```csharp
	var concurrencyLevel = 2;
	var subscriptionWrapper = messageBusSubscriber.CreateBackgroundSemaphoreWithDuplicateEliminationSubscription(async message => { await Task.Delay(2000); Console.WriteLine("Called concurrently"); }, message => message, concurrencyLevel);

	// Create 10 messages
	var messages = new List<SampleMessage>(10);
	for (var i = 0; i < 100; i++)
	{
		messages.Add(new SampleMessage(i % 10));
	}

	// Publish messages to the bus
	for (var i = 0; i < 100; i++)
	{
		messageBusPublisher.PublishAsync(messages[i % 10]);
	}
```
In this example, the duplicate elimination key is the message itself, so the same instance can not be added twice. 


If you want to be able to handle add duplicates to the subscription queue after a message handler has started but before it has finished,
you can couple this subscription with the FireAndForGetSubscription:
```csharp
	var concurrencyLevel = 2;
	var subscriptionWrapper = messageBusSubscriber.CreateBackgroundSemaphoreWithDuplicateEliminationSubscription(new FireAndForgetSubscription(async message => { await Task.Delay(2000); Console.WriteLine("Called concurrently"); }), message => message.Id, concurrencyLevel);

	for (var i = 0; i < 100; i++)
	{
		messageBusPublisher.PublishAsync(new SampleMessage(i % 10));
	}
```

#### Limited Throughput subscription

Example:
```csharp
	// TODO: add example
```

#### Retry subscription
This subscription will automatically retry X times if the handler throws an exception. 
Each retry (not the initial try) is separated by a user specified delay.
If all X attempts fail, the subscription throws an exception with the last exception as inner exception

Example:
```csharp
	
	var bus = new ConcurrentMessageBus<int>();

	// Try invoking the handler 10 times with 500ms in between before propagating the last
    bus.CreateRetrySubscription(
        message =>
            {
				// This handler fails
                throw new Exception("Handler failed");
            }, 
        10,
        TimeSpan.FromMilliseconds(500));
		
```
After 10 attempts, an Exception is thrown with the last handler exception as inner exception


### Publishing
The default method of publishing messages to the subscribers is Parallel. The subscriber methods are invoked in parallel and are then awaited as a group. See ParallelPublisher below.

Publishing is implemented by Publishers, types deriving from BusPublisher<TMessageType>. Serpent.Common.MessageBus has implementation for a lot of common scenarios.

To change publisher, change the BusPublisher property on the ConcurrentMessageBusOptions<TMessageType>, either directly or by using one of the extension methods.

3 examples to use FireAndForgetPublisher:

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