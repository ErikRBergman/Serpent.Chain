namespace Serpent.MessageHandlerChain.Tests.Decorators.Retry
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Exceptions;
    using Serpent.MessageHandlerChain.Models;

    using Xunit;

    public class RetryDecoratorTests
    {
        [Fact]
        public async Task RetryExceptionTests()
        {
            var exception = await Assert.ThrowsAsync<RetryFailedException>(
                                async () =>
                                    {
                                        var count = 1;

                                        var func = Create.SimpleFunc<int>(
                                            b => b.Retry(r => r.MaximumNumberOfAttempts(5).RetryDelay(TimeSpan.FromMilliseconds(1)).OnFail(() => count++))
                                                .Handler(message => throw new Exception(count.ToString())));

                                        await func(0);
                                    });

            Assert.Equal(5, exception.NumberOfAttempts);
            Assert.Equal(TimeSpan.FromMilliseconds(1), exception.Delay);

            Assert.Equal(5, exception.Exceptions.Count);

            Assert.Contains("1", exception.Exceptions.Select(e => e.Message));
            Assert.Contains("2", exception.Exceptions.Select(e => e.Message));
            Assert.Contains("3", exception.Exceptions.Select(e => e.Message));
            Assert.Contains("4", exception.Exceptions.Select(e => e.Message));
            Assert.Contains("5", exception.Exceptions.Select(e => e.Message));
        }

        [Fact]
        public async Task RetryTests()
        {
            var attemptsCount = 0;

            var func = Create.SimpleFunc<int>(
                b => b.FireAndForget()
                    .Retry(
                        r => r.MaximumNumberOfAttempts(5)
                            .RetryDelay(TimeSpan.FromMilliseconds(100))
                            .OnFail(
                                (message, exception, attempt, maxNumberOfAttempts, delay, token) =>
                                    {
                                        Debug.WriteLine(DateTime.Now + $" attempt {attempt} / {maxNumberOfAttempts}");
                                        attemptsCount++;
                                    }))
                    .Handler(message => throw new Exception(DateTime.Now.ToString(CultureInfo.CurrentCulture))));

            await func(0);

            await Task.Delay(900);

            Assert.Equal(5, attemptsCount);
        }

        [Fact]
        public async Task TestRetryHandler()
        {
            var handler = new RetryLogger<int>();

            var func = Create.SimpleFunc<int>(
                b => b
                    .FireAndForget()
                    .Retry(
                        r => r.MaximumNumberOfAttempts(1)
                            .RetryDelay(TimeSpan.FromMilliseconds(100))
                            .RetryHandler(handler))
                    .Handler(message => throw new Exception(DateTime.Now.ToString(CultureInfo.CurrentCulture))));

            await func(0);

            await Task.Delay(900);

            Assert.Equal(1, handler.RetryInvokedCount);

            func = Create.SimpleFunc<int>(
                b => b
                    .Retry(
                        r => r.MaximumNumberOfAttempts(5)
                            .RetryDelay(TimeSpan.FromMilliseconds(100))
                            .RetryHandler(handler))
                    .Handler(message => { }));

            await func(0);

            Assert.Equal(1, handler.SuccessInvokedCount);
        }

        internal class RetryLogger<TMessageType> : IMessageHandlerRetry<TMessageType>
        {
            private int retryInvokedCount;

            private int successInvokedCount;

            public int RetryInvokedCount => this.retryInvokedCount;

            public int SuccessInvokedCount => this.successInvokedCount;

            public Task HandleRetryAsync(FailedMessageHandlingAttempt<TMessageType> attemptInformation)
            {
                Interlocked.Increment(ref this.retryInvokedCount);
                return Task.CompletedTask;
            }

            public Task MessageHandledSuccessfullyAsync(MessageHandlingAttempt<TMessageType> attemptInformation)
            {
                Interlocked.Increment(ref this.successInvokedCount);
                return Task.CompletedTask;
            }
        }
    }
}