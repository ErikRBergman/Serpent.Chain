namespace Serpent.MessageHandlerChain.Tests.Decorators.Distinct
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Interfaces;

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