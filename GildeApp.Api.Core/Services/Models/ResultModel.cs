using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Api.Core.Services.Models
{
    public class ResultModel<T> : BaseResultModel
    {
        public T Data { get; set; }
    }
}
