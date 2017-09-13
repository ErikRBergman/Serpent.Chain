namespace Serpent.Common.Async
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class ParallelQueueWorker<T>
    {
        private readonly int concurrencyLevel;

        private readonly object lockObject = new object();

        private readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();

        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0);

        private readonly Func<T, CancellationToken, Task> workerFunc;

        private CancellationTokenSource cancellationTokenSource;

        private int isStarted;

        private IReadOnlyCollection<Task> workerTasks;

        public ParallelQueueWorker(Func<T, CancellationToken, Task> workerFunc, IEnumerable<T> items = null, int? maxConcurrencyLevel = null)
        {
            maxConcurrencyLevel = maxConcurrencyLevel ?? Environment.ProcessorCount * 2;

            if (maxConcurrencyLevel < 1)
            {
                throw new ArgumentException("maxConcurrencyLevel may not be less than 1");
            }

            this.workerFunc = workerFunc ?? throw new ArgumentNullException(nameof(workerFunc));

            this.concurrencyLevel = maxConcurrencyLevel.Value;
            if (items != null)
            {
                this.EnqueueRange(items);
            }
        }

        public int Count => this.queue.Count;

        public bool IsRunning => this.isStarted == 1;

        public void Enqueue(T item)
        {
            this.queue.Enqueue(item);
            this.semaphoreSlim.Release();
        }

        public void EnqueueRange(IEnumerable<T> items)
        {
            var count = 0;

            foreach (var item in items)
            {
                this.queue.Enqueue(item);
                count++;
            }

            this.semaphoreSlim.Release(count);
        }

        public void Start()
        {
            lock (this.lockObject)
            {
                if (Interlocked.CompareExchange(ref this.isStarted, 1, 0) == 0)
                {
                    this.workerTasks = new List<Task>(this.concurrencyLevel);
                    this.cancellationTokenSource = new CancellationTokenSource();

                    var tasks = new List<Task>(this.concurrencyLevel);

                    for (var i = 0; i < this.concurrencyLevel; i++)
                    {
                        tasks.Add(Task.Run(() => this.WorkerMethodAsync(this.cancellationTokenSource.Token)));
                    }

                    this.workerTasks = tasks;
                }
                else
                {
                    throw new Exception("Worker may not be started more than once");
                }
            }
        }

        public Task StopAsync()
        {
            lock (this.lockObject)
            {
                var tasks = this.workerTasks;
                var tokenSource = this.cancellationTokenSource;

                if (Interlocked.CompareExchange(ref this.isStarted, 0, 0) == 0)
                {
                    this.workerTasks = null;
                    this.cancellationTokenSource = null;

                    tokenSource.Cancel();
                    return Task.WhenAll(tasks);
                }
                else
                {
                    throw new Exception("Worker is not running");
                }
            }
        }

        private async Task WorkerMethodAsync(CancellationToken token)
        {
            while (true)
            {
                await this.semaphoreSlim.WaitAsync(token);

                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (this.queue.TryDequeue(out var item))
                {
                    await this.workerFunc(item, token);
                }
            }
        }
    }
}