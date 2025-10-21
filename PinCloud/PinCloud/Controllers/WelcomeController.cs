using Microsoft.AspNetCore.Mvc;

namespace APinI.Controllers
{
    [ApiController]
    [Route("")]
    public class WellcomeController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Welcome to APinI API";
        }
    }
}