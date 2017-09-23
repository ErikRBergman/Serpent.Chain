using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serpent.Common.MessageBus.Tests.SubscriptionTypes
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    [TestClass]
    public class RetrySubscriptionTests
    {
        [TestMethod]
        public async Task RetryTests()
        {
            var bus = new ConcurrentMessageBus<int>();

            int attemptsCount = 0;

            var sub = bus.Subscribe()
                .FireAndForget()
                .Retry(
                5,
                TimeSpan.FromMilliseconds(100),
                    (message, exception, attempt, maxNumberOfAttempts) =>
                        {
                            Debug.WriteLine(DateTime.Now + $" attempt {attempt} / {maxNumberOfAttempts}");
                            attemptsCount++;
                        })
                .Handler(
                    async message => { throw new Exception(DateTime.Now.ToString(CultureInfo.CurrentCulture)); });


            await bus.PublishAsync();

            await Task.Delay(900);

            Assert.AreEqual(5, attemptsCount);

        }
    }
}