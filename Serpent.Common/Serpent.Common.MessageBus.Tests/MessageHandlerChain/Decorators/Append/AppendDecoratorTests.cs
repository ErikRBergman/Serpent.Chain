// ReSharper disable InconsistentNaming
namespace Serpent.Common.MessageBus.Tests.MessageHandlerChain.Append
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AppendDecoratorTests
    {
        [TestMethod]
        public async Task Append_Normal_Tests()
        {
            var bus = new ConcurrentMessageBus<Message1>();

            var counter = 0;

            using (bus.Subscribe(b => 
                b.Append(msg => msg)
                .Handler(
                    async message =>
                        {
                            Interlocked.Add(ref counter, message.AddValue);
                        })).Wrapper())
            {

                for (var i = 0; i < 100; i++)
                {
                    await bus.PublishAsync(new Message1(1 + (i % 2)));
                }

                Assert.AreEqual(300, counter);
            }

            counter = 0;

            using (bus.Subscribe(b => 
                b.Append(async msg => msg)
                .Handler(
                    async message =>
                        {
                            Interlocked.Add(ref counter, message.AddValue);
                        })).Wrapper())
            {

                for (var i = 0; i < 100; i++)
                {
                    await bus.PublishAsync(new Message1(1 + (i % 2)));
                }

                Assert.AreEqual(300, counter);
            }



        }

        private class Message1
        {
            public Message1(int addValue)
            {
                this.AddValue = addValue;
            }

            public int AddValue { get; set; }
        }
    }
}