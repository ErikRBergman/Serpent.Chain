namespace Serpent.MessageHandlerChain.Helpers
{
    using System.Threading.Tasks;

    internal static class TaskHelper
    {
        public static Task<bool> FalseTask { get; } = Task.FromResult(false);

        public static Task<bool> TrueTask { get; } = Task.FromResult(true);
    }
}