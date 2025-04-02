using Prometheus;
using Server2.Models;

namespace Server2.Services
{
    public class ServerMetricsService
    {
        private readonly ServerState _state = new();
        private readonly Random _random = new();

        private readonly Gauge _cpuUsage = Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
        private readonly Gauge _activeRequests = Metrics.CreateGauge("server_active_requests", "Active Requests");

        public ServerMetricsService()
        {

            new Timer(_ =>
            {
                _state.CpuUsage = _random.Next(10, 90);
                _cpuUsage.Set(_state.CpuUsage);
            }, null, 0, 5000);
        }

        public void IncreaseRequests()
        {
            _state.ActiveRequests++;
            _activeRequests.Set(_state.ActiveRequests);
        }

        public void DecreaseRequests()
        {
            if (_state.ActiveRequests > 0)
                _state.ActiveRequests--;

            _activeRequests.Set(_state.ActiveRequests);
        }

        public ServerState GetServerState()
        {
            return _state;
        }
    }
}
