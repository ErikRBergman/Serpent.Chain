namespace Serpent.Chain.Tests.Helpers
{
    using Serpent.Chain.Helpers;

    using Xunit;

    public class ActionHelpersTests
    {
        [Fact]
        public void TestNoAction()
        {
            var action = ActionHelpers.NoAction;
            action();
        }
    }
}
