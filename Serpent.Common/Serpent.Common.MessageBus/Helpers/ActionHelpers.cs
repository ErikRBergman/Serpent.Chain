namespace Serpent.Common.MessageBus.Helpers
{
    using System;

    public static class ActionHelpers
    {
        public static Action NoAction { get; } = () => { };
    }
}