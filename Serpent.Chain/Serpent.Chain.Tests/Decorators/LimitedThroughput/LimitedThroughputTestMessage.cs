namespace Serpent.Chain.Tests.Decorators.LimitedThroughput
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