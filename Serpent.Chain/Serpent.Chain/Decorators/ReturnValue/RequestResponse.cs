namespace Serpent.Chain.Decorators.ReturnValue
{
    using System.Threading.Tasks;

    public struct RequestResponse<TRequestType, TResponseType>
    {
        public RequestResponse(TRequestType request, TaskCompletionSource<TResponseType> response)
        {
            this.Request = request;
            this.Response = response;
        }

        public TRequestType Request { get; }

        public TaskCompletionSource<TResponseType> Response { get; }
    }
}
