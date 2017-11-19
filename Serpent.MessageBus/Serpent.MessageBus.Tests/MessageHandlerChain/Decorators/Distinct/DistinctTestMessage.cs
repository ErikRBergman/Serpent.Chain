namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Distinct
{
    internal class DistinctTestMessage
    {
        public DistinctTestMessage(string id)
        {
            this.Id = id;
        }

        public string Id { get; }
    }
}