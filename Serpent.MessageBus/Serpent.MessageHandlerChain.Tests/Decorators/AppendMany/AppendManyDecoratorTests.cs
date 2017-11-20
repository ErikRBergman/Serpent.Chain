// ReSharper disable InconsistentNaming

namespace Serpent.MessageHandlerChain.Tests.Decorators.AppendMany
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading.Tasks;

    using Xunit;

    public class AppendManyDecoratorTests
    {
        [Fact]
        public async Task AppendMany_Normal_Tests()
        {
            var items = new ConcurrentBag<int>();

            var func = Create.SimpleFunc<int>(b => b.AppendMany(msg => ImmutableList<int>.Empty.Add(msg + 1).Add(msg + 2).Add(msg + 3)).Handler(msg => items.Add(msg)));

            await func(1);

            Assert.Equal(4, items.Count);

            Assert.Contains(1, items);
            Assert.Contains(2, items);
            Assert.Contains(3, items);
            Assert.Contains(4, items);

            await func(5);
            Assert.Equal(8, items.Count);

            Assert.Contains(5, items);
            Assert.Contains(6, items);
            Assert.Contains(7, items);
            Assert.Contains(8, items);
        }

        [Fact]
        public async Task AppendMany_Predicate_Tests()
        {
            var items = new ConcurrentBag<int>();

            var func = Create.SimpleFunc<int>(
                b => b.AppendMany(c => c.Select(msg => ImmutableList<int>.Empty.Add(msg + 1).Add(msg + 2).Add(msg + 3)).Where(msg => msg % 2 == 0)).Handler(msg => items.Add(msg)));

            await func(1);

            Assert.Single(items);
            Assert.Contains(1, items);

            await func(2);
            Assert.Equal(5, items.Count);

            Assert.Contains(2, items);
            Assert.Contains(3, items);
            Assert.Contains(4, items);
            Assert.Contains(5, items);
        }

        [Fact]
        public async Task AppendMany_Recursive_No_Predicate_Tests()
        {
            var items = new ConcurrentBag<RecursiveObject>();

            var func = Create.SimpleFunc<RecursiveObject>(b => b.AppendMany(c => c.Select(msg => msg.Children).Recursive()).Handler(msg => items.Add(msg)));

            var source = new RecursiveObject
                             {
                                 Id = 1,
                                 Children = new[]
                                                {
                                                    new RecursiveObject
                                                        {
                                                            Id = 11,
                                                            Children = new[]
                                                                           {
                                                                               new RecursiveObject
                                                                                   {
                                                                                       Id = 111
                                                                                   },
                                                                               new RecursiveObject
                                                                                   {
                                                                                       Id = 112
                                                                                   }
                                                                           }
                                                        },
                                                    new RecursiveObject
                                                        {
                                                            Id = 12
                                                        }
                                                }
                             };

            await func(source);

            Assert.Equal(5, items.Count);
            Assert.Contains(items, i => i.Id == 1);
            Assert.Contains(items, i => i.Id == 11);
            Assert.Contains(items, i => i.Id == 111);
            Assert.Contains(items, i => i.Id == 112);
            Assert.Contains(items, i => i.Id == 12);
        }

        [Fact]
        public async Task AppendMany_Recursive_With_Predicate_Tests()
        {
            var items = new ConcurrentBag<RecursiveObject>();

            var func = Create.SimpleFunc<RecursiveObject>(
                b => b.AppendMany(c => c.Select(msg => msg.Children).Where(msg => msg.Id != 12).Recursive()).Handler(msg => items.Add(msg)));

            var source = new RecursiveObject
                             {
                                 Id = 1,
                                 Children = new[]
                                                {
                                                    new RecursiveObject
                                                        {
                                                            Id = 11,
                                                            Children = new[]
                                                                           {
                                                                               new RecursiveObject
                                                                                   {
                                                                                       Id = 111
                                                                                   },
                                                                               new RecursiveObject
                                                                                   {
                                                                                       Id = 112
                                                                                   }
                                                                           }
                                                        },
                                                    new RecursiveObject
                                                        {
                                                            Id = 12,
                                                            Children = new[]
                                                                           {
                                                                               new RecursiveObject
                                                                                   {
                                                                                       Id = 121
                                                                                   },
                                                                               new RecursiveObject
                                                                                   {
                                                                                       Id = 122
                                                                                   }
                                                                           }
                                                        }
                                                }
                             };

            await func(source);

            Assert.Equal(5, items.Count);
            Assert.Contains(items, i => i.Id == 1);
            Assert.Contains(items, i => i.Id == 11);
            Assert.Contains(items, i => i.Id == 111);
            Assert.Contains(items, i => i.Id == 112);
            Assert.Contains(items, i => i.Id == 12);
        }

        private class RecursiveObject
        {
            public IEnumerable<RecursiveObject> Children { get; set; }

            public int Id { get; set; }
        }
    }
}