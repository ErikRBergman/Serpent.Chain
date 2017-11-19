namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.Distinct
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Interfaces;

    [Distinct(nameof(DistinctTestMessage.Id))]
    internal class DistinctMessageHandler : IMessageHandler<DistinctTestMessage>
    {
        public int NumberOfInvokations { get; set; }

        public Task HandleMessageAsync(DistinctTestMessage distinctTestMessage, CancellationToken cancellationToken)
        {
            this.NumberOfInvokations++;

            return Task.CompletedTask;
        }
    }
}