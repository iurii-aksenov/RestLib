using Newtonsoft.Json;

namespace Core.Infrastructure
{
    public record RestError
    {
        [JsonConstructor]
        public RestError(int errorType, string message, string errorCode, string? stackTrace = null)
        {
            ErrorType = errorType;
            Message = message;
            ErrorCode = errorCode;
            StackTrace = stackTrace;
        }

        public int ErrorType { get; }
        public string Message { get; }
        public string ErrorCode { get; }
        public string? StackTrace { get; }
    }
}