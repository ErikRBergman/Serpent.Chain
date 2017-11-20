namespace Serpent.MessageHandlerChain.Decorators.WeakReference
{
    /// <summary>
    /// The weak reference garbage collector interface
    /// </summary>
    public interface IWeakReferenceGarbageCollector
    {
        /// <summary>
        /// Adds a weak reference to the garbage collector to check periodically
        /// </summary>
        /// <param name="weakReference">The weak reference</param>
        void Add(IWeakReferenceGarbageCollection weakReference);
    }
}