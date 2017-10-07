namespace Serpent.Common.MessageBus.MessageHandlerChain.WireUp
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class WireUpAttribute : Attribute
    {
    }
}