using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server1.Services;

namespace Server1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly ServerMetricsService _metricsService;

        public StatusController(ServerMetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(_metricsService.GetServerState());
        }
    }
}
