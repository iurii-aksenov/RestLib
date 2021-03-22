using Newtonsoft.Json;

namespace Core.Infrastructure
{
    public record RestResponse<T>
    {
        public RestResponse(T data)
        {
            Data = data;
        }

        public RestResponse(RestError error)
        {
            Error = error;
        }

        [JsonConstructor]
        public RestResponse(T? data, RestError? error)
        {
            Data = data;
            Error = error;
        }

        public bool HasError => Error != null;
        public T? Data { get; }
        public RestError? Error { get; }

        public static implicit operator T?(RestResponse<T> response)
        {
            return response.Data;
        }
    }
}