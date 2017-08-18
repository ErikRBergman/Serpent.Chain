namespace Serpent.Common.Rotators
{
    public abstract class ReadOnlyRotator<T> : Rotator<T>
    {
        public abstract int Count { get; }
    }
}