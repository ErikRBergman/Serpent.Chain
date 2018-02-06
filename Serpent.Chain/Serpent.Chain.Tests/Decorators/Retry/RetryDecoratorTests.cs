namespace Serpent.Chain.Tests.Decorators.Retry
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Exceptions;
    using Serpent.Chain.Helpers;
    using Serpent.Chain.Models;

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
            Assert.Equal(TimeSpan.FromMilliseconds(1), exception.Delays.First());

            Assert.Equal(5, exception.Exceptions.Count);

            Assert.Contains("1", exception.Exceptions.Select(e => e.Message));
            Assert.Contains("2", exception.Exceptions.Select(e => e.Message));
            Assert.Contains("3", exception.Exceptions.Select(e => e.Message));
            Assert.Contains("4", exception.Exceptions.Select(e => e.Message));
            Assert.Contains("5", exception.Exceptions.Select(e => e.Message));
        }

        [Fact]
        public async Task RetryExceptionAbortAfter2Tests()
        {
            var exception = await Assert.ThrowsAsync<RetryFailedException>(
                                async () =>
                                    {
                                        var count = 1;

                                        var func = Create.SimpleFunc<int>(
                                            b => b.Retry(r => r.MaximumNumberOfAttempts(5).RetryDelay(TimeSpan.FromMilliseconds(1)).OnFail(() =>
                                                    {
                                                        count++;
                                                        return count != 3;
                                                    }))
                                                .Handler(message => throw new Exception(count.ToString())));

                                        await func(0);
                                    });

            Assert.Equal(2, exception.NumberOfAttempts);
            Assert.Equal(TimeSpan.FromMilliseconds(1), exception.Delays.First());
            Assert.Equal(2, exception.Exceptions.Count);

            Assert.Contains("1", exception.Exceptions.Select(e => e.Message));
            Assert.Contains("2", exception.Exceptions.Select(e => e.Message));
        }

        [Fact]
        public async Task RetryExceptionMultipleTimeouts()
        {
            var retryDelays = new[] { TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(300) };

            int count = 0;

            var watch = Stopwatch.StartNew();

            var exception = await Assert.ThrowsAsync<RetryFailedException>(
                                async () =>
                                    {
                                        var func = Create.SimpleFunc<int>(
                                            b => b.Retry(r => r.MaximumNumberOfAttempts(6).RetryDelays(retryDelays).OnFail(attempt =>
                                                    {
                                                        Assert.Equal(retryDelays[count], attempt.Delay);
                                                        count = Math.Min(count + 1, retryDelays.Length - 1);
                                                        return TaskHelper.TrueTask;
                                                    }))
                                                .Handler(message => throw new Exception()));

                                        await func(0);
                                    });

            watch.Stop();

            // It shoud be at least 1200 but the Travis CI build server (or .NET Core) does not guarantee the delay is at least
            Assert.True(watch.ElapsedMilliseconds > 1100, watch.Elapsed.ToString());

            // It shoud be at most 1300 but the Travis CI build server (or .NET Core) does not guarantee the delays
            Assert.True(watch.ElapsedMilliseconds < 1700);

            Assert.Equal(6, exception.NumberOfAttempts);
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
        public async Task RetryWhereTests()
        {
            bool wasTriggered = false;

            var func = Create.SimpleFunc<int>(
                b => b
                    .Retry(
                        r => r.MaximumNumberOfAttempts(5)
                            .RetryDelay(TimeSpan.FromMilliseconds(100))
                            .Where(
                                attempt =>
                                    {
                                        wasTriggered = true;
                                        return false;
                                    }))
                    .Handler(message => throw new Exception(DateTime.Now.ToString(CultureInfo.CurrentCulture))));

            await func(0);

            Assert.True(wasTriggered);

            // Now return false
            wasTriggered = false;

            func = Create.SimpleFunc<int>(
                b => b
                    .Retry(
                        r => r.MaximumNumberOfAttempts(5)
                            .RetryDelay(TimeSpan.FromMilliseconds(100))
                            .Where(
                                attempt =>
                                    {
                                        wasTriggered = true;
                                        return true;
                                    }))
                    .Handler(message => throw new Exception(DateTime.Now.ToString(CultureInfo.CurrentCulture))));

            await Assert.ThrowsAsync<RetryFailedException>(() => func(0));

            Assert.True(wasTriggered);

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

            public Task<bool> HandleRetryAsync(FailedMessageHandlingAttempt<TMessageType> attemptInformation)
            {
                Interlocked.Increment(ref this.retryInvokedCount);
                return TaskHelper.TrueTask;
            }

            public Task MessageHandledSuccessfullyAsync(MessageHandlingAttempt<TMessageType> attemptInformation)
            {
                Interlocked.Increment(ref this.successInvokedCount);
                return Task.CompletedTask;
            }
        }
    }
}