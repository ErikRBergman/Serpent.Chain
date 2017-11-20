namespace Serpent.MessageBus.Helpers
{
    using System.Threading.Tasks;

    public static class TaskHelper
    {
        public static Task<bool> FalseTask { get; } = Task.FromResult(false);

        public static Task<bool> TrueTask { get; } = Task.FromResult(true);
    }
}