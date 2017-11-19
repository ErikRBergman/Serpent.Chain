namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.LimitedThroughput
{
    internal class LimitedThroughputTestMessage
    {
        public LimitedThroughputTestMessage(string id)
        {
            this.Id = id;
        }

        public string Id { get; }
    }
}