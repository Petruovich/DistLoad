using DistLoad.Services;
using Microsoft.AspNetCore.Mvc;

namespace DistLoad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadBalancerController : ControllerBase
    {
        private readonly LoadBalancerManager _manager;

        public LoadBalancerController(LoadBalancerManager manager)
        {
            _manager = manager;
        }

        [HttpGet("servers")]
        public IActionResult GetServers()
        {
            return Ok(_manager.GetServers());
        }

        [HttpGet("algorithm")]
        public IActionResult GetCurrentAlgorithm()
        {
            return Ok(_manager.GetCurrentAlgorithm());
        }

        [HttpPost("algorithm/{algorithm}")]
        public IActionResult ChangeAlgorithm(string algorithm)
        {
            bool result = _manager.SetAlgorithm(algorithm);
            if (!result) return BadRequest("Unknown algorithm");
            return Ok($"Algorithm changed to {algorithm}");
        }
    }
}
