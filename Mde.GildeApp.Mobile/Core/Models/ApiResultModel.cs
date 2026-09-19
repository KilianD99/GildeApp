using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Mobile.Core.Models
{
    public class ApiResult<T>
    {
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public bool IsSuccess { get; set; }
    }
}
