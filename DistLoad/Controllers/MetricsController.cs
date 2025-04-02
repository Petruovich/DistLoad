using Microsoft.AspNetCore.Mvc;
using DistLoad.Metrics;
using Prometheus;

namespace DistLoad.Controllers
{
    [Route("api/metrics")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly PrometheusExporter _metricsExporter;

        public MetricsController(PrometheusExporter metricsExporter)
        {
            _metricsExporter = metricsExporter;
        }

        [HttpGet]
        public IActionResult GetMetrics()
        {
            return Ok(new
            {
                cpuUsage = PrometheusExporter.CpuUsageGauge.Value,       
                activeRequests = PrometheusExporter.ActiveRequestsGauge.Value, 
                totalRequests = PrometheusExporter.TotalRequestsCounter.Value  
            });
        }
    }
}
