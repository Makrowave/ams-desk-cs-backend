namespace ams_desk_cs_backend.Shared.Results
{
    public enum ServiceStatus
    {
        Ok,
        NotFound,
        NoChanges,
        BadRequest,
        NoContent,
        Unauthorized,
    }

    public class ServiceResult
    {
        public ServiceStatus Status { get; set; }
        public string Message { get; set; }

        public ServiceResult(ServiceStatus status, string message)
        {
            Status = status;
            Message = message;
        }

        public static ServiceResult NotFound(string message)
        {
            return new ServiceResult(ServiceStatus.NotFound, message);
        }
        
        public static ServiceResult BadRequest(string message)
        {
            return new ServiceResult(ServiceStatus.BadRequest, message);
        }
    }
    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }

        public ServiceResult(ServiceStatus status, string message, T? data)
            : base(status, message)
        {
            Status = status;
            Message = message;
            Data = data;
        }
        
        public static ServiceResult<T> NotFound(string message)
        {
            return new ServiceResult<T>(ServiceStatus.NotFound, message, default);
        }
        public static ServiceResult<T> BadRequest(string message)
        {
            return new ServiceResult<T>(ServiceStatus.BadRequest, message, default);
        }
    }
}
