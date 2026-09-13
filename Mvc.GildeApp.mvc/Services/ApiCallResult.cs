namespace Mvc.GildeApp.mvc.Services
{
    public class ApiCallResult<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public List<string> Errors { get; init; } = new();

        public static ApiCallResult<T> Ok(T? data) => new() { IsSuccess = true, Data = data };

        public static ApiCallResult<T> Fail(params string[] errors) =>
            new() { IsSuccess = false, Errors = errors.ToList() };

        public static ApiCallResult<T> Fail(IEnumerable<string> errors) =>
            new() { IsSuccess = false, Errors = errors.ToList() };
    }
}

