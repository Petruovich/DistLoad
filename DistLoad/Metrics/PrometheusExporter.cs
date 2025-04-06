using Prometheus;
using System.Net.Http;
using System.Text.Json;
using DistLoad.Models;
//using Server1.Models;

namespace DistLoad.Metrics
{
    public class PrometheusExporter
    {
        public static readonly Gauge CpuUsageGauge =
            global::Prometheus.Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
        public static readonly Gauge ActiveRequestsGauge =
            global::Prometheus.Metrics.CreateGauge("server_active_requests", "Active Requests");
        public static readonly Counter TotalRequestsCounter =
            global::Prometheus.Metrics.CreateCounter("server_total_requests", "Total number of requests");

        private readonly HttpClient _httpClient;
        private readonly List<ServerInstance> _servers;
        private readonly object _lock = new();

        public PrometheusExporter(List<ServerInstance> servers)
        {
            _httpClient = new HttpClient();
            _servers = servers;

            Task.Run(UpdateMetricsLoop);
            Task.Run(LogMetricsLoop);
        }

        public async Task UpdateMetricsLoop()
        {
            while (true)
            {
                await UpdateMetrics();
                await Task.Delay(5000); 
            }
        }

        public async Task LogMetricsLoop()
        {
            while (true)
            {
                LogMetrics();
                await Task.Delay(5000); 
            }
        }

        public async Task UpdateMetrics()
        {
            foreach (var server in _servers)
            {
                try
                {
                    var response = await _httpClient.GetStringAsync($"{server.Address}/api/status");
                    //var status = JsonSerializer.Deserialize<ServerState>(response);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var status = JsonSerializer.Deserialize<ServerState>(response, options);


                    if (status != null)
                    {
                        lock (_lock)
                        {
                            CpuUsageGauge.Set(status.CpuUsage /*ToString()*/);
                            ActiveRequestsGauge.Set(status.ActiveRequests);
                            TotalRequestsCounter.Inc(); 
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Помилка отримання метрик з {server.Address}: {ex.Message}");
                }
            }
        }

        public void LogMetrics()
        {
            Console.Clear();
            Console.WriteLine(" [Метрики серверів] - Оновлення кожні 5 секунд:\n");

            foreach (var server in _servers)
            {
                Console.WriteLine($" Сервер: {server.Address}");
                Console.WriteLine($"    CPU Usage: {CpuUsageGauge.Value}%");
                Console.WriteLine($"    Active Requests: {ActiveRequestsGauge.Value}");
                Console.WriteLine($"    Total Requests: {TotalRequestsCounter.Value}");
                Console.WriteLine(new string('-', 40));
            }
        }
    }
}