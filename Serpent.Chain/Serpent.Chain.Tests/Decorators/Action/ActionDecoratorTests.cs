// ReSharper disable InconsistentNaming

namespace Serpent.Chain.Tests.Decorators.Action
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Helpers;

    using Xunit;

    public class ActionDecoratorTests
    {
        [Fact]
        public void Test_Action_CancelThroughOperationCanceledException()
        {
            var counter = 0;

            var ranBefore = 0;
            var ranCancelled = 0;
            var threwException = 0;
            var ranFinally = 0;
            var ranSuccessfully = 0;

            var func = Create.SimpleFunc<int>(
                b => b.Action(
                        a => a.Before(_ => ranBefore = ++counter)
                            .OnCancel(_ => ranCancelled = ++counter)
                            .OnException(_ => threwException = ++counter)
                            .Finally(_ => ranFinally = ++counter)
                            .OnSuccess(_ => ranSuccessfully = ++counter))
                    .Handler(_ => throw new OperationCanceledException()));

            func(0);

            Assert.Equal(1, ranBefore);
            Assert.Equal(2, ranCancelled);
            Assert.Equal(0, threwException);
            Assert.Equal(0, ranSuccessfully);
            Assert.Equal(3, ranFinally);
        }

        [Fact]
        public void Test_Action_CancelThroughToken()
        {
            var counter = 0;

            var ranBefore = 0;
            var ranCancelled = 0;
            var threwException = 0;
            var ranFinally = 0;
            var ranSuccessfully = 0;

            var func = Create.Func<int>(
                b => b.Action(
                        a => a.Before(_ => ranBefore = ++counter)
                            .OnCancel(_ => ranCancelled = ++counter)
                            .OnException(_ => threwException = ++counter)
                            .Finally(_ => ranFinally = ++counter)
                            .OnSuccess(_ => ranSuccessfully = ++counter))
                    .Handler(_ => throw new Exception("Fail!")));

            using (var tokenSource = new CancellationTokenSource())
            {
                tokenSource.Cancel();

                func(0, tokenSource.Token);

                Assert.Equal(1, ranBefore);
                Assert.Equal(2, ranCancelled);
                Assert.Equal(0, threwException);
                Assert.Equal(0, ranSuccessfully);
                Assert.Equal(3, ranFinally);
            }
        }

        [Fact]
        public async Task Test_Action_Exception_Inhibit_Rethrow()
        {
            var counter = 0;

            var ranBefore = 0;
            var ranCancelled = 0;
            var threwException = 0;
            var ranFinally = 0;
            var ranSuccessfully = 0;

            var func = Create.SimpleFunc<int>(
                b => b.Action(
                        a => a.Before(_ => ranBefore = ++counter)
                            .OnCancel(_ => ranCancelled = ++counter)
                            .OnException((msg, exception) =>
                                {
                                    threwException = ++counter;
                                    return TaskHelper.FalseTask;
                                })
                            .Finally(_ => ranFinally = ++counter)
                            .OnSuccess(_ => ranSuccessfully = ++counter))
                    .Handler(_ => throw new Exception("Fail!")));

            await func(0);

            Assert.Equal(1, ranBefore);
            Assert.Equal(0, ranCancelled);
            Assert.Equal(2, threwException);
            Assert.Equal(0, ranSuccessfully);
            Assert.Equal(3, ranFinally);
        }

        [Fact]
        public async Task Test_Action_Exception_Inhibit()
        {
            var counter = 0;

            var ranBefore = 0;
            var ranCancelled = 0;
            var threwException = 0;
            var ranFinally = 0;
            var ranSuccessfully = 0;

            var func = Create.SimpleFunc<int>(
                b => b.Action(
                        a => a.Before(_ => ranBefore = ++counter)
                            .OnCancel(_ => ranCancelled = ++counter)
                            .OnException(_ => threwException = ++counter)
                            .Finally(_ => ranFinally = ++counter)
                            .OnSuccess(_ => ranSuccessfully = ++counter))
                    .Handler(_ => throw new Exception("Fail!")));

            await Assert.ThrowsAsync<Exception>(() => func(0));

            Assert.Equal(1, ranBefore);
            Assert.Equal(0, ranCancelled);
            Assert.Equal(2, threwException);
            Assert.Equal(0, ranSuccessfully);
            Assert.Equal(3, ranFinally);
        }

        [Fact]
        public void Test_Action_Normal()
        {
            var counter = 0;

            var ranBefore = 0;
            var ranCancelled = 0;
            var threwException = 0;
            var ranFinally = 0;
            var ranSuccessfully = 0;

            var func = Create.SimpleFunc<int>(
                b => b.Action(
                        a => a.Before(_ => ranBefore = ++counter)
                            .OnCancel(_ => ranCancelled = ++counter)
                            .OnException(_ => threwException = ++counter)
                            .Finally(_ => ranFinally = ++counter)
                            .OnSuccess(_ => ranSuccessfully = ++counter))
                    .Handler(_ => { }));

            func(0);

            Assert.Equal(1, ranBefore);
            Assert.Equal(0, ranCancelled);
            Assert.Equal(0, threwException);
            Assert.Equal(2, ranSuccessfully);
            Assert.Equal(3, ranFinally);
        }
    }
}