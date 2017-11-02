namespace Serpent.MessageBus.Helpers
{
    using System;

    internal static class ActionHelpers
    {
        public static Action NoAction { get; } = () => { };
    }
}