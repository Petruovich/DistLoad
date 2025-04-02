using Microsoft.AspNetCore.Mvc;
using Server3.Services;

namespace Server3.Controllers
{
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
