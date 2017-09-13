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
