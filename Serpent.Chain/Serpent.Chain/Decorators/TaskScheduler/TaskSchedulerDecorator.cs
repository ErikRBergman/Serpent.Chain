namespace Serpent.Chain.Decorators.TaskScheduler
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TaskSchedulerDecorator<TMessageType> : ChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly TaskScheduler taskScheduler;

        public TaskSchedulerDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, TaskScheduler taskScheduler)
        {
            this.handlerFunc = handlerFunc;
            this.taskScheduler = taskScheduler;
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            var task = Task.Factory.StartNew(
                () => this.handlerFunc(message, token), 
                token,
                TaskCreationOptions.None, 
                this.taskScheduler).Unwrap();
            return task;
        }
    }
}