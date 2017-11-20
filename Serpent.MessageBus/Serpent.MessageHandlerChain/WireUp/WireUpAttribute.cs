namespace Serpent.MessageHandlerChain.WireUp
{
    using System;

    /// <summary>
    /// Provides a common base for Wire Up attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class WireUpAttribute : Attribute
    {
    }
}