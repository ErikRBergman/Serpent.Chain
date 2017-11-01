// ReSharper disable InconsistentNaming

namespace Serpent.Common.MessageBus.Tests.MessageHandlerChain.Retry
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry;

    [TestClass]
    public class RetryWireUpTests
    {
        [TestMethod]
        public async Task RetryWireUp_RetryHandler_Test()
        {
            var bus = new ConcurrentMessageBus<Message>();

            var handler = new RetryMessageHandler();

            bus.Subscribe(b => b.SoftFireAndForget().WireUp(handler));

            await bus.PublishAsync();

            await Task.Delay(400);

            Assert.AreEqual(3, handler.Attempts.Count);
        }

        [TestMethod]
        public async Task RetryWireUp_Test()
        {
            var bus = new ConcurrentMessageBus<Message>();

            var handler = new NonRetryMessageHandler();

            bus
                .Subscribe(b => b
                .SoftFireAndForget()
                .WireUp(handler));

            await bus.PublishAsync();

            await Task.Delay(400);

            Assert.AreEqual(3, handler.NumberOfInvokations);
        }

        private class Message
        {
        }

        [Retry(3, "00:00:00.100", UseIMessageHandlerRetry = true)]
        private class NonRetryMessageHandler : IMessageHandler<Message>
        {
            public int NumberOfInvokations { get; set; }

            public Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
            {
                this.NumberOfInvokations++;
                throw new NotImplementedException();
            }
        }

        [Retry(3, "00:00:00.100", UseIMessageHandlerRetry = true)]
        private class RetryMessageHandler : IMessageHandler<Message>, IMessageHandlerRetry<Message>
        {
            public List<int> Attempts { get; } = new List<int>();

            public Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public Task HandleRetryAsync(Message message, Exception exception, int attemptNumber, int maxNumberOfAttemps, TimeSpan delay, CancellationToken token)
            {
                this.Attempts.Add(attemptNumber);
                return Task.CompletedTask;
            }

            public Task MessageHandledSuccessfullyAsync(Message message, int attemptNumber, int maxNumberOfAttemps, TimeSpan delay)
            {
                return Task.CompletedTask;
            }
        }
    }
}