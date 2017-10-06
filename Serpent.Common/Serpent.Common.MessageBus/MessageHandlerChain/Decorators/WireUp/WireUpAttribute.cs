namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class WireUpAttribute : Attribute
    {
    }
}