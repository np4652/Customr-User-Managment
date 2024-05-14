namespace UserManagement.Domain.Base
{
    public interface IResponse
    {
        ResponseStatus StatusCode { get; set; }
        string ResponseText { get; set; }
    }

    public class Response : IResponse
    {
        public ResponseStatus StatusCode { get; set; }
        public string ResponseText { get; set; }
        public Response()
        {
            StatusCode = ResponseStatus.Fail;
            ResponseText = ResponseStatus.Failed.ToString();
        }
    }

    public interface IResponse<T> : IResponse
    {
        T Result { get; set; }
    }

    public class Response<T> : Response, IResponse<T>
    {
        public T Result { get; set; }
        public Response()
        {
            StatusCode = ResponseStatus.Fail;
            ResponseText = ResponseStatus.Failed.ToString();
        }

        public Response(T result)
        {
            StatusCode = ResponseStatus.Success;
            ResponseText = ResponseStatus.Success.ToString();
            Result = result;
        }
    }

    public interface IRequest
    {
        string AuthToken { get; set; }
    }

    public interface IRequest<T> : IRequest
    {
        T Param { get; set; }
    }

    public class Request : IRequest
    {
        public string AuthToken { get; set; }
    }

    public class Request<T> : Request, IRequest<T>
    {
        public T Param { get; set; }
    }

    public interface IServiceRequest
    {
        int LognId { get; set; }
        int RoleId { get; set; }
    }

    public interface IServiceRequest<T>: IServiceRequest
    {
        T param { get; set; }
    }

    public class ServiceRequest : IServiceRequest
    {
        public int LognId { get; set; }
        public int RoleId { get; set; }
    }

    public class ServiceRequest<T> : ServiceRequest , IServiceRequest<T>
    {
        public T param { get ; set ; }
    }

    public interface IRow { }
    public interface IColumn { }
    public interface IFilter { }
}
