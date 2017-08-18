namespace Serpent.Common.Rotators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class StaticRotator<T> : ReadOnlyRotator<T>
    {
        private readonly T[] items;

        private readonly int itemCount;

        private int currentIndex = -1;

        public StaticRotator(IEnumerable<T> items)
        {
            this.items = items.ToArray();
            this.itemCount = this.items.Length;
        }

        public override int Count => this.itemCount;

        public override T GetNext()
        {
            if (this.items.Length == 0)
            {
                return default(T);
            }

            var index = Interlocked.Increment(ref this.currentIndex);
            return this.items[index % this.itemCount];
        }
    }
}