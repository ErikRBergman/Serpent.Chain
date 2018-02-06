// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.Append
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Xunit;

    public class MoreAppendDecoratorTests
    {
        [Fact]
        public async Task AppendDecoratorTest_NoPredicate()
        {
            var items = new List<MyMessage>();

            var func = Create.SimpleFunc<MyMessage>(
                b => b.Append(
                        m => m.Select(
                            im => new MyMessage
                            {
                                Id = 2
                            }))
                    .Handler(
                        msg =>
                            {
                                lock (items)
                                {
                                    items.Add(msg);
                                }
                            }));

            await func(
                new MyMessage
                {
                    Id = 1
                });

            Assert.Equal(2, items.Count);

            Assert.Equal(1, items[0].Id);
            Assert.Equal(2, items[1].Id);
        }

        [Fact]
        public async Task AppendDecoratorTest_Normal()
        {
            var items = new List<int>();

            var func = Create.SimpleFunc<int>(
                b => b.Append(msg => 1)
                    .Handler(
                        msg =>
                            {
                                lock (items)
                                {
                                    items.Add(msg);
                                }
                            }));

            await func(0);

            Assert.Equal(2, items.Count);

            Assert.Equal(0, items[0]);
            Assert.Equal(1, items[1]);
        }

        [Fact]
        public async Task AppendDecoratorTest_Predicate()
        {
            var items = new List<MyMessage>();

            var func = Create.SimpleFunc<MyMessage>(
                b => b.Append(
                        ib => ib.Where(msg => msg.Id == 1)
                            .Select(
                                msg => new MyMessage
                                {
                                    Id = msg.Id + 1
                                }))
                    .Handler(
                        msg =>
                            {
                                lock (items)
                                {
                                    items.Add(msg);
                                }
                            }));

            await func(
                new MyMessage
                {
                    Id = 1
                });

            Assert.Equal(2, items.Count);

            Assert.Equal(1, items[0].Id);
            Assert.Equal(2, items[1].Id);
        }

        [Fact]
        public async Task AppendDecoratorTest_Subscribe_Async()
        {
            var items = new List<MyMessage>();

            var func = Create.SimpleFunc<MyMessage>(
                b => b.Append(
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

            await func(
                new MyMessage
                {
                    Id = 1
                });

            Assert.Equal(2, items.Count);

            Assert.Equal(1, items[0].Id);
            Assert.Equal(2, items[1].Id);
        }

        [Fact]
        public async Task AppendDecoratorTest_Subscribe_Normal_Async()
        {
            var items = new List<int>();

            var func = Create.SimpleFunc<int>(
                b => b.Append(
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

            await func(0);

            Assert.Equal(2, items.Count);

            Assert.Equal(0, items[0]);
            Assert.Equal(1, items[1]);
        }

        [Fact]
        public async Task AppendDecoratorTest_Subscribe_Predicate_Recursive()
        {
            var items = new List<MyMessage>();

            var func = Create.SimpleFunc<MyMessage>(
                b => b.Append(ib => ib.Recursive().Where(msg => msg.InnerMessage != null).Select(msg => msg.InnerMessage))
                    .Handler(
                        msg =>
                            {
                                lock (items)
                                {
                                    items.Add(msg);
                                }
                            }));

            await func(
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
            var items = new List<MyMessage>();

            var func = Create.SimpleFunc<MyMessage>(
                b => b.Append(ib => ib.Recursive().Where(msg => msg.InnerMessage != null).Select(msg => msg.InnerMessage))
                    .Handler(
                        msg =>
                            {
                                lock (items)
                                {
                                    items.Add(msg);
                                }
                            }));

            await func(
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

        private class MyMessage
        {
            public int Id { get; set; }

            public MyMessage InnerMessage { get; set; }
        }
    }
}