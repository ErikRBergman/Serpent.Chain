﻿// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    internal static class XTodoExtensions
    {
        //// Other types
        //// .Collect(int maxMessages, TimeSpan? maxTime)  / .Collect(TimeSpan time) - queue messages and release a read only collection of the messages queued when the threshold is reached

        //// LINQ TODO
        ////  Aggregate - Maybe with TimeSpan and/or number of messagse
        ////  Append - Add a secondary message when a message is received
        ////  Cast - Cast messages to specified type
        ////  Contains - Like SQL Where IN, low prio
        ////  Distinct - Only a single message may only pass through once based on key selector
        ////  Except - Like SQL WHERE NOT IN
        ////  First - Only a single message, either by itself or based on a predicate
        ////  FirstOrDefault - A single message based on predicate with a TimeSpan to stop listening
        ////  OfType - Only messages of a specified type
        ////  OrderBy - probably not relevant
        ////  OrderByDescending - probably not relevant
        ////  Prepend - Insert message before the current message
        ////  Repeat - Repeat a message X times (maybe after a TimeSpan)
        ////  SelectMany - Interesting bus.Subscribe().SelectMany(message => message.InnerMessages).SoftFireAndForget().Concurrent(16)...
        ////  Skip - Skip the first X messages (based on predicate or not)
        ////  SkipWhile - Cool
        ////  Take - First X messages - optionally based on TimeSpan
        ////  TakeWhile - Subscribe while the predicate is fullilled

        //// LINQ Done
        ////  Select - Done
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

        public static IMessageHandlerChainBuilder<TMessageType> Where<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> asyncPredicate)
        {
            if (asyncPredicate == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(innerMessageHandler =>
                {
                    return async message =>
                        {
                            if (await asyncPredicate(message))
                            {
                                await innerMessageHandler(message);
                            }
                        };
                });
        }

        public static IMessageHandlerChainBuilder<TMessageType> Where<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> predicate)
        {
            if (predicate == null)
            {
                return messageHandlerChainBuilder;
            }

            return messageHandlerChainBuilder.Add(innerMessageHandler => message =>
                {
                    if (predicate(message))
                    {
                        return innerMessageHandler(message);
                    }

                    return Task.CompletedTask;
                });
        }

    }
}