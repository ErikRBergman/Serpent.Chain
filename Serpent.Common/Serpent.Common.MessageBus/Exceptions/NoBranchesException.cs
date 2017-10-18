namespace Serpent.Common.MessageBus.Exceptions
{
    using System;

    public class NoBranchesException : Exception
    {
        public NoBranchesException(string message)
            : base(message)
        {
        }
    }
}