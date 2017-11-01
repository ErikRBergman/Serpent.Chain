namespace Serpent.Common.MessageBus.Tests.MessageHandlerChain.Retry
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RetryDecoratorTests
    {
        [TestMethod]
        public async Task RetryTests()
        {
            var bus = new ConcurrentMessageBus<int>();

            var attemptsCount = 0;

            using (bus.Subscribe(b => b
                .FireAndForget()
                .Retry(
                    5,
                    TimeSpan.FromMilliseconds(100),
                    (message, exception, attempt, maxNumberOfAttempts, delay, token) =>
                        {
                            Debug.WriteLine(DateTime.Now + $" attempt {attempt} / {maxNumberOfAttempts}");
                            attemptsCount++;
                        })
                .Handler(message => throw new Exception(DateTime.Now.ToString(CultureInfo.CurrentCulture))))
                .Wrapper())
            {
                await bus.PublishAsync();

                await Task.Delay(900);

                Assert.AreEqual(5, attemptsCount);
            }
        }
    }
}