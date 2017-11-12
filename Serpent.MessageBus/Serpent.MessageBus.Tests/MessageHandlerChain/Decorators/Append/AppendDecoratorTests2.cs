// ReSharper disable InconsistentNaming

namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Append
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Serpent.MessageBus;

    using Xunit;

    public class AppendDecoratorTests2
    {
        [Fact]
        public async Task AppendDecoratorTest_Subscribe_Normal()
        {
            var bus = new ConcurrentMessageBus<int>();
            var items = new List<int>();

            bus.Subscribe(b =>
                b.Append(msg => 1)
                .Handler(
                    msg =>
                        {
                            lock (items)
                            {
                                items.Add(msg);
                            }
                        }));

            await bus.PublishAsync(0);

            Assert.Equal(2, items.Count);

            Assert.Equal(0, items[0]);
            Assert.Equal(1, items[1]);
        }

        [Fact]
        public async Task AppendDecoratorTest_Subscribe_Normal_Async()
        {
            var bus = new ConcurrentMessageBus<int>();
            var items = new List<int>();

            bus.Subscribe(b =>
                b.Append(
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
                        }));

            await bus.PublishAsync(0);

            Assert.Equal(2, items.Count);

            Assert.Equal(0, items[0]);
            Assert.Equal(1, items[1]);
        }

        [Fact]
        public async Task AppendDecoratorTest_Subscribe_Predicate()
        {
            var bus = new ConcurrentMessageBus<MyMessage>();
            var items = new List<MyMessage>();

            bus.Subscribe(b => b
                .Append(
                    ib =>
                        ib.Where(msg => msg.Id == 1).Select(msg => new MyMessage { Id = msg.Id + 1 }))
                .Handler(
                    msg =>
                        {
                            lock (items)
                            {
                                items.Add(msg);
                            }
                        }));

            await bus.PublishAsync(
                new MyMessage
                {
                    Id = 1
                });

            Assert.Equal(2, items.Count);

            Assert.Equal(1, items[0].Id);
            Assert.Equal(2, items[1].Id);
        }

        [Fact]
        public async Task AppendDecoratorTest_Subscribe_Predicate_Recursive()
        {
            var bus = new ConcurrentMessageBus<MyMessage>();
            var items = new List<MyMessage>();

            bus.Subscribe(
                b => b
                    .Append(ib => ib.Recursive().Where(msg => msg.InnerMessage != null).Select(msg => msg.InnerMessage))
                    .Handler(
                        msg =>
                            {
                                lock (items)
                                {
                                    items.Add(msg);
                                }
                            }));

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

            Assert.Equal(3, items.Count);

            Assert.Equal(1, items[0].Id);
            Assert.Equal(2, items[1].Id);
            Assert.Equal(3, items[2].Id);
        }

        [Fact]
        public async Task AppendDecoratorTest_Subscribe_Predicate_Recursive_Async()
        {
            var bus = new ConcurrentMessageBus<MyMessage>();
            var items = new List<MyMessage>();

            bus.Subscribe(b => b
                .Append(ib => ib.Recursive().Where(msg => msg.InnerMessage != null).Select(msg => msg.InnerMessage))
                .Handler(
                    msg =>
                        {
                            lock (items)
                            {
                                items.Add(msg);
                            }
                        }));

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

            Assert.Equal(3, items.Count);

            Assert.Equal(1, items[0].Id);
            Assert.Equal(2, items[1].Id);
            Assert.Equal(3, items[2].Id);
        }

        [Fact]
        public async Task AppendDecoratorTest_Subscribe_Async()
        {
            var bus = new ConcurrentMessageBus<MyMessage>();
            var items = new List<MyMessage>();

            bus.Subscribe(b =>
                b.Append(
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
                        }));

            await bus.PublishAsync(
                new MyMessage
                {
                    Id = 1
                });

            Assert.Equal(2, items.Count);

            Assert.Equal(1, items[0].Id);
            Assert.Equal(2, items[1].Id);
        }

        private class MyMessage
        {
            public int Id { get; set; }

            public MyMessage InnerMessage { get; set; }
        }
    }
}