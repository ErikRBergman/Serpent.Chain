namespace Serpent.Common.Async
{
    using System;
    using System.Threading.Tasks;

    public class LazyAsync<T>
    {
        private readonly Func<Task<T>> loadValueFunc;

        private Task<T> cachedValue;

        public LazyAsync(Func<Task<T>> loadValueFunc)
        {
            this.loadValueFunc = loadValueFunc;
            this.cachedValue = (Task<T>)null;
        }

        /// <summary>
        /// Gets the value
        /// </summary>
        public Task<T> ValueAsync
        {
            get
            {
                var value = this.cachedValue;

                if (value != null)
                {
                    return value;
                }
                
                // This is to prevent the load method from being called multiple times.
                // Yes, locking on yourself should be avoided, but to save memory we lock on ourselves for now.
                lock (this)
                {
                    return this.cachedValue ?? (this.cachedValue = this.loadValueFunc());
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                this.cachedValue = (Task<T>)null;
            }
        }
    }
}