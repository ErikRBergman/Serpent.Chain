using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serpent.Common.MessageBus.Tests
{
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.MessageHandlerChain;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.SoftFireAndForget;

    [TestClass]
    public class Experiments
    {
        private class TestAttribute : Attribute
        {
            public string PropertyName { get; }

            public TestAttribute(string propertyName)
            {
                this.PropertyName = propertyName;
            }
        }

        [Test(nameof(Id))]

        public class TestMessage
        {
            public string Id { get; set; }
        }

        [SoftFireAndForget]
        [Delay("00:10:00")]
        [Retry(5, "00:01:00", UseIMessageHandlerRetry = true)]
        [Concurrent(16)]
        public class TestMessageHandler : IMessageHandler<TestMessage>, IMessageHandlerRetry<TestMessage>
        {
            public async Task HandleMessageAsync(TestMessage message, CancellationToken cancellationToken)
            {
            }

            public async Task HandleRetryAsync(TestMessage message, Exception exception, int attemptNumber, int maxNumberOfAttemps, TimeSpan delay, CancellationToken cancellationToken)
            {
            }

            public async Task MessageHandledSuccessfullyAsync(TestMessage message, int attemptNumber, int maxNumberOfAttemps, TimeSpan delay)
            {
            }
        }

        private void DoIt<T>(T instance)
        {
            var type = typeof(T);

            var attribute = type.GetCustomAttributes(true).FirstOrDefault(a => a is TestAttribute) as TestAttribute;

            if (attribute == null)
            {
                throw new Exception($"No \"Test\" attribute found on type {type.Name}");
            }

            var property = type.GetProperties().FirstOrDefault(p => p.Name == attribute.PropertyName);
            if (property == null)
            {
                throw new Exception($"Property {attribute.PropertyName} not found");
            }

            var getValue = Expression.Call(property.GetMethod, Expression.Parameter(type));
            //var lambda = Expression.Lambda<T, object>(getValue);
            //var func = lambda.Compile();
            

            // Creating a label to jump to from a loop.  
            //LabelTarget label = Expression.Label(property.PropertyType);



        }

        [TestMethod]
        public void TestMethod1()
        {

            var instance = new TestMessage();

            this.DoIt<TestMessage>(instance);






        }
    }
}
