using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Mobile.Core.Services
{
    public class ApiCall<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public string Error { get; init; } = string.Empty;
        public bool IsConflict { get; init; }

        public static ApiCall<T> Ok(T? data) => new() { IsSuccess = true, Data = data };

        public static ApiCall<T> Fail(string error) =>
            new() { IsSuccess = false, Error = error };

        public static ApiCall<T> Conflict(string error) =>
            new() { IsSuccess = false, Error = error, IsConflict = true };
    }
}
