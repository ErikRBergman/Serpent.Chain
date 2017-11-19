namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Distinct
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Interfaces;

    [Distinct]
    internal class DistinctValueTypeMessageHandler : IMessageHandler<int>
    {
        public int NumberOfInvokations { get; set; }

        public Task HandleMessageAsync(int message, CancellationToken cancellationToken)
        {
            this.NumberOfInvokations++;

            return Task.CompletedTask;
        }
    }
}