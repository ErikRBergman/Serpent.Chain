namespace Serpent.Common.MessageBus.Helpers
{
    internal static class ExclusiveAccessExtensions
    {
        public static int Increment(this ExclusiveAccess<int> exclusiveAccess)
        {
            return exclusiveAccess.Update(v => ++v);
        }
    }
}