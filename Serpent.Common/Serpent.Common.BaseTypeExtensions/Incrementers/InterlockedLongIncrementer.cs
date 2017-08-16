namespace Serpent.Common.BaseTypeExtensions.Incrementers
{
    using System.Threading;

    public class InterlockedLongIncrementer
    {
        private long count = 0;

        public long Count => this.count;

        public long Increment()
        {
            return Interlocked.Increment(ref this.count);
        }

        public long Reset(long? newValue = null)
        {
            newValue = newValue ?? 0;
            return Interlocked.Exchange(ref this.count, newValue.Value);
        }
    }
}