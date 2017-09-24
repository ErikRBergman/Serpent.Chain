////namespace Serpent.Common.MessageBus.SubscriptionTypes
////{
////    using System;
////    using System.Threading.Tasks;

////    public class LimitedThroughputSubscription<TMessageType> : BusSubscription<TMessageType>
////    {
////        private readonly Func<TMessageType, Task> handlerFunc;

////        private readonly BusSubscription<TMessageType> innerSubscription;

////        private readonly int maxMessagesPerPeriod;

////        private readonly TimeSpan periodSpan;

////        private volatile int periodMessageCount = 0;

////        private object lockObject = new object();

////        private DateTime periodStart = DateTime.MinValue;

////        private Task waitTask = Task.CompletedTask;

////        public LimitedThroughputSubscription(BusSubscription<TMessageType> innerSubscription, int maxMessagesPerPeriod, TimeSpan periodSpan)
////        {
////            this.innerSubscription = innerSubscription;
////            this.maxMessagesPerPeriod = maxMessagesPerPeriod;

////            if (this.maxMessagesPerPeriod <= 0)
////            {
////                throw new ArgumentException("Max messages per period may not be 0 or less");
////            }

////            this.periodSpan = periodSpan;
////            this.handlerFunc = innerSubscription.HandleMessageAsync;
////        }

////        public LimitedThroughputSubscription(Func<TMessageType, Task> handlerFunc, int maxMessagesPerPeriod, TimeSpan periodSpan)
////        {
////            this.handlerFunc = handlerFunc;
////            this.maxMessagesPerPeriod = maxMessagesPerPeriod;

////            if (this.maxMessagesPerPeriod <= 0)
////            {
////                throw new ArgumentException("Max messages per period may not be 0 or less");
////            }

////            this.periodSpan = periodSpan;
////        }

////        public override async Task HandleMessageAsync(TMessageType message)
////        {
////            Task lockTask;

////            lock (this.lockObject)
////            {
////                lockTask = this.waitTask;
////            }

////            await lockTask.ConfigureAwait(false);

////            Task periodEndWaitTask = null;

////            lock (this.lockObject)
////            {
////                var now = DateTime.UtcNow;
////                var diff = (this.periodStart + this.periodSpan) - now;
////                if (diff < TimeSpan.Zero)
////                {
////                    this.ResetPeriodUnsafe(1);
////                }
////                else
////                {
////                    this.periodMessageCount++;
////                    if (this.periodMessageCount > this.maxMessagesPerPeriod)
////                    {
////                        // We will have to await the next period start
////                        periodEndWaitTask = this.waitTask = this.WaitAndReset(diff);
////                    }
////                }
////            }

////            if (periodEndWaitTask != null)
////            {
////                await periodEndWaitTask.ConfigureAwait(false);
////            }

////            await this.handlerFunc(message).ConfigureAwait(false);
////        }

////        private void ResetPeriodUnsafe(int count = 0)
////        {
////            this.periodStart = DateTime.UtcNow;
////            this.periodMessageCount = count;
////        }

////        private async Task WaitAndReset(TimeSpan time)
////        {
////            await Task.Delay(time);

////            lock (this.lockObject)
////            {
////                this.ResetPeriodUnsafe(1);
////            }
////        }
////    }
////}