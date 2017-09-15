namespace Serpent.Common.Async
{
    using System;
    using System.Threading.Tasks;

    public class LazyAsyncWithTimeout<T>
    {
        private readonly Func<Task<T>> loadValueFunc;

        private readonly object lockObject = new object();

        private readonly bool resetTimeoutOnGet;

        private readonly TimeSpan timeToLive;

        private Task<T> value = Task.FromResult(default(T));

        private DateTime loadTime = DateTime.MinValue;

        public LazyAsyncWithTimeout(Func<Task<T>> loadValueFunc, TimeSpan timeToLive, bool resetTimeoutOnGet = true)
        {
            this.loadValueFunc = loadValueFunc;
            this.timeToLive = timeToLive;
            this.resetTimeoutOnGet = resetTimeoutOnGet;
        }

        public bool IsLoadRequired
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.value == null || DateTime.Now - this.loadTime > this.timeToLive;
                }
            }
        }

        public Task<T> ValueAsync
        {
            get
            {
                lock (this.lockObject)
                {
                    var now = DateTime.Now;
                    if (this.value == null || now - this.loadTime > this.timeToLive)
                    {
                        this.loadTime = now;
                        return this.value = this.loadValueFunc.Invoke();
                    }

                    if (this.resetTimeoutOnGet)
                    {
                        this.loadTime = now;
                    }

                    return this.value;
                }
            }
        }

        public void Clear(bool onlyIfLoadIsRequired = false)
        {
            lock (this.lockObject)
            {
                if (onlyIfLoadIsRequired == false || DateTime.Now - this.loadTime > this.timeToLive)
                {
                    this.loadTime = DateTime.MinValue;
                    this.value = null;
                }
            }
        }
    }
}