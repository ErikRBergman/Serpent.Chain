// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.Retry
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Interfaces;
    using Serpent.Chain.Models;

    using Xunit;

    public class RetryWireUpTests
    {
        [Fact]
        public async Task RetryWireUp_RetryHandler_Test()
        {
            var handler = new RetryMessageHandler();

            var func = Create.SimpleFunc<Message>(b => b.SoftFireAndForget().WireUp(handler));

            await func(new Message());

            await Task.Delay(400);

            Assert.Equal(3, handler.Attempts.Count);
        }

        [Fact]
        public async Task RetryWireUp_Test()
        {
            var handler = new NonRetryMessageHandler();

            var func = Create.SimpleFunc<Message>(b => b.SoftFireAndForget().WireUp(handler));

            await func(new Message());

            await Task.Delay(400);

            Assert.Equal(3, handler.NumberOfInvokations);
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

            public Task HandleRetryAsync(FailedMessageHandlingAttempt<Message> attemptInformation)
            {
                this.Attempts.Add(attemptInformation.AttemptNumber);
                return Task.CompletedTask;
            }

            public Task MessageHandledSuccessfullyAsync(MessageHandlingAttempt<Message> attemptInformation)
            {
                return Task.CompletedTask;
            }
        }
    }
}