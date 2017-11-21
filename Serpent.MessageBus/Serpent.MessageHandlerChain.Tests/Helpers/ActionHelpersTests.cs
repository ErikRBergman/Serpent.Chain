namespace Serpent.MessageHandlerChain.Tests.Helpers
{
    using Serpent.MessageHandlerChain.Helpers;

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
