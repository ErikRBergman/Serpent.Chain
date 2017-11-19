// ReSharper disable InconsistentNaming

// ReSharper disable UnusedVariable
namespace Serpent.MessageBus.Tests.Helpers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Helpers;

    using Xunit;

    public class ExclusiveAccessTests
    {
        [Fact]
        public async Task ExclusiveAccess_Test()
        {
            var exclusiveAccess0 = new ExclusiveAccess<int>();
            var exclusiveAccess1 = new ExclusiveAccess<int>(1);

            Assert.Equal(0, exclusiveAccess0.Value);
            Assert.Equal(1, exclusiveAccess1.Value);

            exclusiveAccess0.Update(u => 2);
            Assert.Equal(2, exclusiveAccess0.Value);

            var t1 = Task.Run(
                () => exclusiveAccess0.Update(
                    v =>
                        {
                            Thread.Sleep(100);
                            return 10;
                        }));

            await Task.Delay(10);

            Assert.Equal(10, exclusiveAccess0.Value);

            var t2 = Task.Run(
                () => exclusiveAccess0.Update(
                    v =>
                        {
                            Thread.Sleep(100);
                            return 20;
                        }));

            await Task.Delay(20);

            exclusiveAccess0.Use(v => Assert.Equal(20, v));
        }
    }
}