namespace Serpent.MessageHandlerChain.Decorators.WeakReference
{
    /// <summary>
    /// The IWeakReferenceGarbageCollection interface
    /// </summary>
    public interface IWeakReferenceGarbageCollection
    {
        /// <summary>
        /// Disposes the subscription if the handler was reclaimed by garbage collection. 
        /// Returns true if the object was garbage collected.
        /// </summary>
        /// <returns>true if the object was garbage collected and it's safe to remove all references to this object. </returns>
        bool DisposeIfReclamiedByGarbageCollection();
    }
}