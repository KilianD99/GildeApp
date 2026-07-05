using GildeApp.Api.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuleSetController : ControllerBase
    {
        protected readonly IRuleSetService _ruleSetService;

        public RuleSetController(IRuleSetService ruleSetService)
        {
            _ruleSetService = ruleSetService;
        }

        
    }
}
