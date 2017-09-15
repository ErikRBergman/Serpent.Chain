# Serpent.Common.MessageBus
This is an asynchronous .NET Standard 2.0 message bus for usage in any project where a message bus is suitable.

## Example
Using a message bus will make your components coupled together only by the message bus.

Let's say you have a service where you want to store a customer. A dependency injected class could look like this:

```csharp
public class CustomerService : ICustomerService
{
    private readonly ICustomerStorage customerStorage;

    public CustomerService(ICustomerStorage customerStorage)
    {
        this.customerStorage = customerStorage;
    }
    
    public async Task DoSomethingWithACustomerAsync(Customer customer)
    {
        customer.Log.Add("Was in contact with Customer Services");
        await this.customerStorage.UpdateAsync(customer);
    }
}
```

CustomerService is coupled with ICustomerStorage. It has it's uses, but if we don't want CustomerService to know anything about the CustomerStorage at all and just do it's thing and then send a messsage, than potentially can be received by 0 to many subscribers, we use a message bus. 

Our CustomerService and CustomerUpdateMessage classes will look like this instead:

```csharp
public class CustomerUpdatedMessage
{
    public CustomerUpdatedMessage(Customer customer)
    {
        this.Customer = customer;
    }
 
    public Customer Customer { get; }
    
    public DateTime UpdateTimestamp { get; } = DateTime.UtcNow;
}

public class CustomerService : ICustomerService
{
    private readonly IMessageBusPublisher<CustomerUpdatedMessage> customerUpdatedBus;

    public CustomerService(IMessageBusPublisher<CustomerUpdatedMessage> customerUpdatedBus)
    {
        this.customerUpdatedBus = customerUpdatedBus;
    }
    
    public async Task DoSomethingWithACustomerAsync(Customer customer)
    {
        customer.Log.Add("Was in contact with Customer Services");
        await this.customerUpdatedBus.PublishAsync(new CustomerUpdatedMessage(customer));
    }
}
```

With this setup, we can configure the MessageBus to handle the subscriptions in any way we want.

Example subcriber

```csharp
public class CustomerStorage
{
    private readonly SubscriptionWrapper<CustomerUpdatedMessage> customerUpdatedSubscription;

    public CustomerService(IMessageBusSubscriber<CustomerUpdatedMessage> customerUpdatedBus)
    {
        this.customerUpdatedSubscription = customerUpdatedBus.Subscribe(CustomerUpdatedHandler, null);
    }
    
    public async Task CustomerUpdatedHandler(CustomerUpdatedMessage message)
    {
        // Do whatever we like with the updated customer message
    }
}
```

## Customizations

### Publishing and handling messages
You have a selection of options to customize the way messages are handled. You can customize the way messages are published to the subscribers, and you can customize the way the subscribers handle the messages.
Customizing publishing affects all messages being published, while customizing the subscriptions affects only that subscription.

My recommendation would be to use custom subscriptions before custom publishing, since it''s it will not change as much 

### Customizing subscriptions

#### Normal subscription
Just call the Subscribe method.

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



#### Customizing publishing
If you change the way messages are published, naturally it applies to all messages published to the bus.

##### Simple publish options
Changing ConcurrentMessageBus<T>.ConcurrentMessageBusOptions.PublishType can change the way messages are published

```csharp
public enum PublishTypeType
    {
        /// <summary>
        /// A message is sent to the handler functions serially, 
        /// which means the first subscriber must finish handling the message before the second handler is invoked.
        /// The Task returned by PublishAsync is done when all handlers Tasks are done
        /// </summary>
        Serial,

        /// <summary>
        /// A message is sent to all handler functions serially, not awaiting each handler to finish before the next handler is invoked
        /// This is the DEFAULT value. If all handlers are written to follow the TPL guidelines (never blocking), this is usually the best option. 
        /// The Task returned by PublishAsync is done when all handlers Tasks are done
        /// </summary>
        Parallel,

        /// <summary>
        /// A message is sent to all handler functions in parallel, not awaiting each handler to finish before the next handler is invoked
        /// This is the DEFAULT value. If all handlers are written to follow the TPL guidelines (never blocking), this is usually the best option.
        /// The Task returned by PublishAsync is done when all handlers Tasks are done
        /// </summary>
        ForcedParallel,

        /// <summary>
        /// Use this option to use the CustomBusPublisher property for a custom publisher
        /// </summary>
        Custom
    }
```

##### Custom Bus Publishers

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

##### ForcedParallelPublisher (used when specifying PublishTypeType.ForcedParallel)
The type used when PublishTypeType.ForcedParallel is specified.

##### ParallelPublisher (used when specifying PublishTypeType.Parallel)
The type used when PublishTypeType.Parallel is specified.

##### SemaphorePublisher (Decorator)
This concurrency level decides the number messages the bus can publish simultaneously. 

The difference between SemaphorePublisher and BackgroundSemaphorePublisher is that for SemaphorePublisher, PublishAsync does not return before all subscribers have been called.
The mechanisms behind them are different. Use SemaphorePublisher to LIMIT the number of concurrent messages being handled.

The publisher use ParallelPublisher<T> by default for publishing messages but this can be customized through the constructor.

##### SerialPublisher (used when specifying PublishTypeType.Serial)
The type used when PublishTypeType.Serial is specified.


#### Implement your own BusPublisher
Inhert from BusPublisher<T>, use PublishTypeType.Custom and set CustomBusPublisher to your new type.



### Strong and weak references



## Advanced topics




### Subscription handler factories
Instead of having a single(ton) instance handling all messages you can setup a subscription handler factory that instantates a new handler for each message being published to the bus



### Semaphore handler by key