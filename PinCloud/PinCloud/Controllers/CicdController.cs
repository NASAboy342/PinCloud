using APinI.Models;
using APinI.Schedular.Jobs;
using APinI.Services;
using Microsoft.AspNetCore.Mvc;

namespace APinI.Controllers
{
    [ApiController]
    [Route("api/CicdController")]
    public class CicdController : ControllerBase
    {
        private readonly ICicdService _cicdService;
        private readonly IPowerShellService _powerShellService;

        public CicdController(ICicdService cicdService, IPowerShellService powerShellService)
        {
            _cicdService = cicdService;
            _powerShellService = powerShellService;
        }

        // [HttpGet("release-earth-fe")]
        // public async Task<string> Deploy(string scriptFilePath)
        // {
        //     try
        //     {
        //         var script = System.IO.File.ReadAllText(scriptFilePath);
        //         _powerShellService.ValidateScript(script);
        //         var request = new PowerShellRequest { Script = script };
        //         return await _powerShellService.RunPowerShellScript(request);
        //     }
        //     catch (Exception ex)
        //     {
        //         return $"Error: {ex}";
        //     }
        // }
    }
}
