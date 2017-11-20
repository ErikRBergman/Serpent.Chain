namespace Serpent.MessageHandlerChain.Tests.Decorators.Distinct
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Interfaces;

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