// ReSharper disable InconsistentNaming
namespace Serpent.MessageHandlerChain.Tests.Decorators.TaskScheduler
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Xunit;

    public class TaskSchedulerDecoratorTests
    {
        [Fact]
        public async Task TaskScheduler_Tests()
        {
            TaskScheduler currentTaskScheduler = null;
            TaskScheduler defaultTaskScheduler = null;

            var ts = new MyTaskScheduler();

            var noSchedulerFunc = Create.SimpleFunc<int>(s => s.Handler(m => defaultTaskScheduler = TaskScheduler.Current));
            var ourSchedulerFunc = Create.SimpleFunc<int>(s => s.DispatchOnTaskScheduler(ts).Handler(m => currentTaskScheduler = TaskScheduler.Current));

            await noSchedulerFunc(0);
            await ourSchedulerFunc(0);

            Assert.NotEqual(ts, defaultTaskScheduler);
            Assert.Equal(ts, currentTaskScheduler);
        }

        private class MyTaskScheduler : TaskScheduler
        {
            private ConcurrentBag<Task> Tasks { get;  } = new ConcurrentBag<Task>();

            protected override IEnumerable<Task> GetScheduledTasks()
            {
                return this.Tasks;
            }

            protected override void QueueTask(Task task)
            {
                this.TryExecuteTask(task);
                this.Tasks.Add(task);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                this.TryExecuteTask(task);
                return false;
            }
        }
    }
}