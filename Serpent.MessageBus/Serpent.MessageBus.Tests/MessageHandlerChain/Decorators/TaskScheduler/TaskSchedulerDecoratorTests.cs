// ReSharper disable InconsistentNaming
namespace Serpent.MessageBus.Tests.MessageHandlerChain.Decorators.TaskScheduler
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
            var bus = new ConcurrentMessageBus<int>();

            TaskScheduler currentTaskScheduler = null;
            TaskScheduler defaultTaskScheduler = null;

            var ts = new MyTaskScheduler();

            bus.Subscribe(s => s.Handler(m => defaultTaskScheduler = TaskScheduler.Current));
            bus.Subscribe(s => s.DispatchOnTaskScheduler(ts).Handler(m => currentTaskScheduler = TaskScheduler.Current));

            await bus.PublishAsync();

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