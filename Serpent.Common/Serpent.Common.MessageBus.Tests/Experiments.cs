namespace Serpent.Common.MessageBus.Tests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.MessageHandlerChain;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry;
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    [TestClass]
    public class Experiments
    {
        [TestMethod]
        public void TestMethod1()
        {




            //.Where(m => m.Name == "Distinct");

            var instance = new TestMessage();


            var handlerType = typeof(TestMessageHandler);
            var attribute = handlerType.GetCustomAttributes(true).FirstOrDefault(a => a is TestAttribute) as TestAttribute;

            if (attribute == null)
            {
                throw new Exception($"No \"Test\" attribute found on type {handlerType.Name}");
            }



        }


        public class TestMessage
        {
            public string Id { get; set; }
        }

        [SoftFireAndForget]
        [Test(nameof(TestMessage.Id))]
        [Delay("00:10:00")]
        [Retry(5, "00:01:00", UseIMessageHandlerRetry = true)]
        [Concurrent(16)]
        public class TestMessageHandler : IMessageHandler<TestMessage>, IMessageHandlerRetry<TestMessage>
        {
            public async Task HandleMessageAsync(TestMessage message, CancellationToken cancellationToken)
            {
            }

            public async Task HandleRetryAsync(
                TestMessage message,
                Exception exception,
                int attemptNumber,
                int maxNumberOfAttemps,
                TimeSpan delay,
                CancellationToken cancellationToken)
            {
            }

            public async Task MessageHandledSuccessfullyAsync(TestMessage message, int attemptNumber, int maxNumberOfAttemps, TimeSpan delay)
            {
            }
        }

        private class TestAttribute : Attribute
        {
            public TestAttribute(string propertyName)
            {
                this.PropertyName = propertyName;
            }

            public string PropertyName { get; }
        }
    }
}