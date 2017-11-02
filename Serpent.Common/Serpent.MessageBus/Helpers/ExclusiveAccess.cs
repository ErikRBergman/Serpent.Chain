namespace Serpent.MessageBus.Helpers
{
    using System;

    internal class ExclusiveAccess<T>
    {
        private readonly object lockObject = new object();

        private T value;

        public ExclusiveAccess()
        {
        }

        public ExclusiveAccess(T initialValue)
        {
            this.value = initialValue;
        }

        public T Value
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.value;
                }
            }
        }

        public static implicit operator T(ExclusiveAccess<T> value)
        {
            return value.Value;
        }

        public void Use(Action<T> useAction)
        {
            lock (this.lockObject)
            {
                useAction(this.value);
            }
        }

        public T Update(Func<T, T> updateFunc)
        {
            lock (this.lockObject)
            {
                return this.value = updateFunc(this.value);
            }
        }
    }
}