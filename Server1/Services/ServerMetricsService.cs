using Prometheus;
using Server1.Models;
using System;
using System.Threading;

namespace Server1.Services
{
    public class ServerMetricsService
    {
        private readonly ServerState _state = new();
        private readonly Random _random = new();

        private readonly Gauge _cpuUsage = Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
        private readonly Gauge _activeRequests = Metrics.CreateGauge("server_active_requests", "Active Requests");

        public ServerMetricsService()
        {
            //new Timer(_ =>
            //{
            //    _state.CpuUsage = _random.Next(10, 90);
            //    _cpuUsage.Set(*_state.CpuUsage*/);
            //}, null, 0, 5000);
            new Timer(_ =>
            {
               var de = _state.CpuUsage = _random.Next(10, 90);
                _cpuUsage.Set(de/*_state.CpuUsage*/);
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
            _state.CpuUsage = _cpuUsage.Value;  // 🔹 Додаємо актуальне значення
            _state.ActiveRequests = (int)_activeRequests.Value;  // 🔹 Додаємо кількість активних запитів
            return _state;
        }

    }
}
