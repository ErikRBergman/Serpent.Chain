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
        public static Expression<Func<T>> GetExpression<T>(Expression<Func<T>> e)
        {
            return e;
        }


        internal class X
        {
            public int Id { get; set; } 
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            var bus = new ConcurrentMessageBus<TestMessage>();
            var x = new X();

            var e1 = GetExpression(() => x.Id);
            var e2 = GetExpression(() => x.Id);

            var eq = ReferenceEquals(e1, e2);

            {
                bus.Subscribe(builder => builder.Handler(new TestMessageHandler()));
            }

            GC.Collect(2, GCCollectionMode.Forced);
            GC.Collect(2, GCCollectionMode.Forced);
            GC.Collect(2, GCCollectionMode.Forced);
            GC.Collect(2, GCCollectionMode.Forced);

            await Task.Delay(TimeSpan.FromSeconds(60));




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
            ~TestMessageHandler()
            {
                System.Diagnostics.Debug.WriteLine("finalized");
            }


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