namespace Serpent.Common.BaseTypeExtensions.Incrementers
{
    using System.Threading;

    public class InterlockedIntIncrementer
    {
        private int count = 0;

        public int Count => this.count;

        public int Increment()
        {
            return Interlocked.Increment(ref this.count);
        }

        public int Reset(int? newValue = null)
        {
            newValue = newValue ?? 0;
            return Interlocked.Exchange(ref this.count, newValue.Value);
        }
    }
}