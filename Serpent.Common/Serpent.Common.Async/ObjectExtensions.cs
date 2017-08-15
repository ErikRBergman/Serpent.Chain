namespace Serpent.Common.Async
{
    using System;
    using System.Threading.Tasks;

    public static class ObjectExtensions
    {
        public static Task ToTask<T>(this T @object, Func<T, Task> func)
        {
            return Task.Run(() => func(@object));
        }

        public static Task ToTask<T>(this T @object, Action<T> action)
        {
            return Task.Run(() => action(@object));
        }

        public static Task<TReturnValue> ToTask<T, TReturnValue>(this T @object, Func<T, Task<TReturnValue>> func)
        {
            return Task.Run(() => func(@object));
        }

        public static Task<TReturnValue> ToTask<T, TReturnValue>(this T @object, Func<T, TReturnValue> func)
        {
            return Task.Run(() => func(@object));
        }


        public static void x()
        {
            1.ToTask(i => Console.WriteLine(i.ToString()));

        }
    }
}