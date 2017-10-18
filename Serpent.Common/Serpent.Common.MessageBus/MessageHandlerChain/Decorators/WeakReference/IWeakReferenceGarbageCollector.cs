namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WeakReference
{
    public interface IWeakReferenceGarbageCollector
    {
        void Add(IWeakReferenceGarbageCollection weakReference);
    }
}