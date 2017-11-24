namespace Serpent.Chain.Tests.Helpers
{
    using System.Threading.Tasks;

    using Serpent.Chain.Helpers;

    using Xunit;

    public class TaskHelperTests
    {
        [Fact]
        public async Task TestTrueTask()
        {
            Assert.True(await TaskHelper.TrueTask);
        }

        [Fact]
        public async Task TestFalseTask()
        {
            Assert.False(await TaskHelper.FalseTask);
        }
    }
}