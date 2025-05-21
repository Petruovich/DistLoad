using Microsoft.AspNetCore.Mvc;
using DistLoad.Metrics;
using Prometheus;

namespace DistLoad.Controllers
{
    [Route("api/metrics")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly MetricsLogger _metricsExporter;

        public MetricsController(MetricsLogger metricsExporter)
        {
            _metricsExporter = metricsExporter;
        }

        [HttpGet]
        public IActionResult GetMetrics()
        {
            return Ok(new
            {
                cpuUsage = MetricsLogger.CpuUsageGauge.Value,       
                activeRequests = MetricsLogger.ActiveRequestsGauge.Value, 
                totalRequests = MetricsLogger.TotalRequestsCounter.Value  
            });
        }
    }
}
