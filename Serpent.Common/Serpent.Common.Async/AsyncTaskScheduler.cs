namespace Serpent.Common.Async
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncTaskScheduler
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly Func<CancellationToken, Task> funcToExecute;

        private readonly TimeSpan interval;

        private readonly DateTime start;

        public AsyncTaskScheduler(DateTime start, TimeSpan interval, Func<Task> funcToExecute)
        {
            this.start = start;
            this.interval = interval;
            this.funcToExecute = token => funcToExecute();
        }

        public AsyncTaskScheduler(DateTime start, TimeSpan interval, Func<CancellationToken, Task> funcToExecute)
        {
            this.start = start;
            this.interval = interval;
            this.funcToExecute = funcToExecute;
        }

        public void Start()
        {
            Task.Run(
                async () =>
                    {
                        var cancellationToken = this.cancellationTokenSource.Token;

                        while (cancellationToken.IsCancellationRequested == false)
                        {
                            var waitingTime = this.GetDelayTime();
                            await Task.Delay(waitingTime, cancellationToken);

                            // This method should be invoked without await to make the timer accurate
#pragma warning disable 4014
                            Task.Run(() => this.funcToExecute(cancellationToken), cancellationToken);
#pragma warning restore 4014

                            // Make sure some time has passed
                            await Task.Delay(10, cancellationToken);
                        }
                    });
        }

        public void Stop()
        {
            this.cancellationTokenSource.Cancel();
        }

        private TimeSpan GetDelayTime()
        {
            if (this.start > DateTime.Now)
            {
                return this.start - DateTime.Now;
            }

            // Get the time aligned to the interval
            return this.interval - TimeSpan.FromTicks((DateTime.Now - this.start).Ticks % this.interval.Ticks);
        }
    }
}