// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.ReturnValue;

    public static class ReturnValueExtensions
    {
        public static IChainBuilder<RequestResponse<TMessageType, TReturnType>> HandlerWithResult<TMessageType, TReturnType>(this IChainBuilder<RequestResponse<TMessageType, TReturnType>> chainBuilder, Func<TMessageType, CancellationToken, Task<TReturnType>> handlerFunc)
        {
            return chainBuilder.Handler(async (msg, token) =>
                {
                    try
                    {
                        var result = await handlerFunc(msg.Request, token);
                        msg.Response.SetResult(result);
                    }
                    catch (Exception e)
                    {
                        msg.Response.SetException(e);
                    }
                });
        }

        public static IChainBuilder<RequestResponse<TMessageType, TReturnType>> HandlerWithResult<TMessageType, TReturnType>(this IChainBuilder<RequestResponse<TMessageType, TReturnType>> chainBuilder, Func<TMessageType, TReturnType> handlerFunc)
        {
            return chainBuilder.Handler((msg, token) =>
                {
                    try
                    {
                        var result = handlerFunc(msg.Request);
                        msg.Response.SetResult(result);
                    }
                    catch (Exception e)
                    {
                        msg.Response.SetException(e);
                    }

                    return Task.CompletedTask;
                });
        }
    }
}