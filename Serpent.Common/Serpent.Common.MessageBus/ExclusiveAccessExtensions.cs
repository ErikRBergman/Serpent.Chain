namespace Serpent.Common.MessageBus
{
    public static class ExclusiveAccessExtensions
    {
        public static int Increment(this ExclusiveAccess<int> exclusiveAccess)
        {
            return exclusiveAccess.Update(v => ++v);
        }
    }
}