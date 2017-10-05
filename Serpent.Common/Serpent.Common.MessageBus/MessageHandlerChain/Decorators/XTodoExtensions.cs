// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    internal static class XTodoExtensions
    {
        //// Other types
        //// .Collect(int maxMessages, TimeSpan? maxTime)  / .Collect(TimeSpan time) - queue messages and release a read only collection of the messages queued when the threshold is reached

        //// LINQ TODO

        ////  Cast - Cast messages to specified type

        ////  Repeat - Repeat a message X times (maybe after a TimeSpan)

        //// LINQ low prio

        //// LINQ  maybe
        ////  Aggregate - Maybe with TimeSpan and/or number of messagse
        ////  Except - Like SQL WHERE NOT IN
        ////  Contains - Like SQL Where IN, low prio
        ////  OrderBy - probably not relevant
        ////  OrderByDescending - probably not relevant

        //// LINQ Done
        ////  Append - Add a secondary message when a message is received
        ////  Distinct - Only a single message may only pass through once based on key selector
        ////  First - Only a single message, either by itself or based on a predicate
        ////  OfType - Only messages of a specified type
        ////  Prepend - Insert message before the every message
        ////  Select - Done
        ////  SelectMany - Interesting bus.Subscribe().SelectMany(message => message.InnerMessages).SoftFireAndForget().Concurrent(16)...
        ////  Skip - Skip the first X messages (based on predicate or not)
        ////  SkipWhile - Cool
        ////  Take - First X messages
        ////  TakeWhile - Subscribe while the predicate is fullilled
        ////  Where - Done

        //// LINQ N/A
        ////  All - N/A
        ////  Any - N/A
        ////  AsEnumerable - N/A
        ////  Average - N/A
        ////  Concat - Low prio
        ////  Count - N/A
        ////  DefaultIfEmpty - N/A
        ////  ElementAt - N/A
        ////  ElementAtOrDefault - N/A
        ////  FirstOrDefault - N/A A single message based on predicate with a TimeSpan to stop listening
        ////  Empty - N/A
        ////  GroupBy - N/A
        ////  GroupJoin N/A
        ////  Intersect N/A
        ////  Join N/A
        ////  Last N/A
        ////  LastOrDefault N/A
        ////  LongCount /NA
        ////  Max - N/A
        ////  Min - N/A
        ////  Range - N/A
        ////  Reverse - N/A
        ////  SequenceEqual - N/A
        ////  Single - N/A
        ////  SingleOrDefault - N/A
        ////  SkipLast - N/A
        ////  Sum - N/A
        ////  TakeLast - N/A
        ////  ThenBy - N/A
        ////  ThenByDescending - N/A
        ////  ToArray - N/A
        ////  ToDictionary - N/A
        ////  ToHashSet - N/A
        ////  ToList - N/A
        ////  ToLookup - N/A
        ////  Union - N/A
        ////  Zip -  N /A
    }
}
