// ReSharper disable InconsistentNaming

namespace Serpent.Common.MessageBus.Tests.MessageHandlerChain
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AppendDecoratorTests
    {
        [TestMethod]
        public async Task AppendDecoratorTest_Subscribe_Normal()
        {
            var bus = new ConcurrentMessageBus<int>();
            var items = new List<int>();

            bus.Subscribe()
                .Append(msg => 1)
                .Handler(
                    msg =>
                        {
                            lock (items)
                            {
                                items.Add(msg);
                            }
                        });

            await bus.PublishAsync(0);

            Assert.AreEqual(2, items.Count);

            Assert.AreEqual(0, items[0]);
            Assert.AreEqual(1, items[1]);
        }

        [TestMethod]
        public async Task AppendDecoratorTest_Subscribe_Normal_Async()
        {
            var bus = new ConcurrentMessageBus<int>();
            var items = new List<int>();

            bus.Subscribe()
                .Append(
                    async msg =>
                        {
                            await Task.Delay(10);
                            return 1;
                        })
                .Handler(
                    msg =>
                        {
                            lock (items)
                            {
                                items.Add(msg);
                            }
                        });

            await bus.PublishAsync(0);

            Assert.AreEqual(2, items.Count);

            Assert.AreEqual(0, items[0]);
            Assert.AreEqual(1, items[1]);
        }

        [TestMethod]
        public async Task AppendDecoratorTest_Subscribe_Predicate()
        {
            var bus = new ConcurrentMessageBus<MyMessage>();
            var items = new List<MyMessage>();

            bus.Subscribe()
                .Append(
                    msg => msg.Id == 1,
                     msg =>
                        {
                            return new MyMessage
                            {
                                Id = msg.Id + 1
                            };
                        })
                .Handler(
                    msg =>
                        {
                            lock (items)
                            {
                                items.Add(msg);
                            }
                        });

            await bus.PublishAsync(
                new MyMessage
                {
                    Id = 1
                });

            Assert.AreEqual(2, items.Count);

            Assert.AreEqual(1, items[0].Id);
            Assert.AreEqual(2, items[1].Id);
        }

        [TestMethod]
        public async Task AppendDecoratorTest_Subscribe_Predicate_Recursive()
        {
            var bus = new ConcurrentMessageBus<MyMessage>();
            var items = new List<MyMessage>();

            bus.Subscribe()
                .Append(
                    msg => msg.InnerMessage != null,
                    msg => msg.InnerMessage,
                    true)
                .Handler(
                    msg =>
                        {
                            lock (items)
                            {
                                items.Add(msg);
                            }
                        });

            await bus.PublishAsync(
                new MyMessage
                {
                    Id = 1,
                    InnerMessage = new MyMessage
                    {
                        Id = 2,
                        InnerMessage = new MyMessage
                        {
                            Id = 3
                        }
                    }
                });

            Assert.AreEqual(3, items.Count);

            Assert.AreEqual(1, items[0].Id);
            Assert.AreEqual(2, items[1].Id);
            Assert.AreEqual(3, items[2].Id);
        }

        [TestMethod]
        public async Task AppendDecoratorTest_Subscribe_Predicate_Recursive_Async()
        {
            var bus = new ConcurrentMessageBus<MyMessage>();
            var items = new List<MyMessage>();

            bus.Subscribe()
                .Append(
                    msg => Task.FromResult(msg.InnerMessage != null),
                    msg => Task.FromResult(msg.InnerMessage),
                    true)
                .Handler(
                    msg =>
                        {
                            lock (items)
                            {
                                items.Add(msg);
                            }
                        });

            await bus.PublishAsync(
                new MyMessage
                {
                    Id = 1,
                    InnerMessage = new MyMessage
                    {
                        Id = 2,
                        InnerMessage = new MyMessage
                        {
                            Id = 3
                        }
                    }
                });

            Assert.AreEqual(3, items.Count);

            Assert.AreEqual(1, items[0].Id);
            Assert.AreEqual(2, items[1].Id);
            Assert.AreEqual(3, items[2].Id);
        }

        [TestMethod]
        public async Task AppendDecoratorTest_Subscribe_Predicate_Async()
        {
            var bus = new ConcurrentMessageBus<MyMessage>();
            var items = new List<MyMessage>();

            bus.Subscribe()
                .Append(
                    async msg =>
                        {
                            await Task.Delay(10);
                            return new MyMessage
                            {
                                Id = msg.Id + 1
                            };
                        })
                .Handler(
                    msg =>
                        {
                            lock (items)
                            {
                                items.Add(msg);
                            }
                        });

            await bus.PublishAsync(
                new MyMessage
                {
                    Id = 1
                });

            Assert.AreEqual(2, items.Count);

            Assert.AreEqual(1, items[0].Id);
            Assert.AreEqual(2, items[1].Id);
        }

        private class MyMessage
        {
            public int Id { get; set; }

            public MyMessage InnerMessage { get; set; }
        }
    }
}