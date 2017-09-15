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

### Custom subscriptions


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