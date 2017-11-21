namespace Serpent.MessageHandlerChain.Tests.Helpers
{
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Helpers;

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