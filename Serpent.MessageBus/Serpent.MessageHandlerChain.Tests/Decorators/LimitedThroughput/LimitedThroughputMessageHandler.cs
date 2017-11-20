namespace Serpent.MessageHandlerChain.Tests.Decorators.LimitedThroughput
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Interfaces;

    [LimitedThroughput(10, "00:00:01")]
    internal class LimitedThroughputMessageHandler : IMessageHandler<LimitedThroughputTestMessage>
    {
        public int Count { get; private set; }

        public Task HandleMessageAsync(LimitedThroughputTestMessage _, CancellationToken cancellationToken)
        {
            this.Count++;

            return Task.CompletedTask;
        }
    }
}