using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Api.Core.Services.Models
{
    public abstract class BaseResultModel
    {
        public bool IsSuccess => !Errors.Any();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
