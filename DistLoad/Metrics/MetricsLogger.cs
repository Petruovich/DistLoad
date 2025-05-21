//using Prometheus;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Net.Http;
//using System.Text.Json;
//using DistLoad.Models;

//namespace DistLoad.Metrics
//{
//    public class MetricsLogger
//    {
//        private readonly List<ServerInstance> _servers;
//        private readonly object _lock = new();

//        // для зовнішнього Prometheus
//        public static readonly Gauge CpuUsageGauge = global::Prometheus.Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
//        public static readonly Gauge ActiveRequestsGauge = global::Prometheus.Metrics.CreateGauge("server_active_requests", "Active Requests");
//        public static readonly Counter TotalRequestsCounter = global::Prometheus.Metrics.CreateCounter("server_total_requests", "Total requests");

//        public MetricsLogger(List<ServerInstance> servers)
//        {
//            _servers = servers;
//            var t = new Thread(Loop) { IsBackground = true };
//            t.Start();
//        }

//        public void Loop()
//        {
//            while (true)
//            {
//                LogAll();
//                Thread.Sleep(5000);
//            }
//        }

//        private void LogAll()
//        {
//            lock (_lock)
//            {
//                // Блок 1: Метрики серверів
//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine("=== Server Metrics (every 5s) ===");
//                foreach (var s in _servers)
//                {
//                    Console.WriteLine($"• {s.Id} @ {s.Address}");
//                    Console.WriteLine($"    CPU Usage:       {CpuUsageGauge.Value}%");
//                    Console.WriteLine($"    Active Requests: {ActiveRequestsGauge.Value}");
//                    Console.WriteLine($"    Total Requests:  {TotalRequestsCounter.Value}");
//                }

//                // Блок 2: Логи балансувальника
//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.WriteLine("\n=== Load Balancer Dispatch Counts ===");
//                foreach (var kv in LoadBalancerMiddleware.ServerCounters)
//                {
//                    Console.WriteLine($"Server {kv.Key}: {kv.Value} requests");
//                }

//                Console.ResetColor();
//                Console.WriteLine(); // відступ для читабельності
//            }
//        }
//    }
//}




//using Prometheus;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text.Json;
//using System.Threading;
//using System.Threading.Tasks;
//using DistLoad.Models;

//namespace DistLoad.Metrics
//{
//    public class MetricsLogger
//    {
//        private readonly List<ServerInstance> _servers;
//        private readonly HttpClient _http = new();
//        private readonly object _lock = new();

//        // ці ж Gauge використовуються для зовнішнього Prometheus
//        public static readonly Gauge CpuUsageGauge = global::Prometheus.Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
//        public static readonly Gauge ActiveRequestsGauge = global::Prometheus.Metrics.CreateGauge("server_active_requests", "Active Requests");
//        public static readonly Counter TotalRequestsCounter = global::Prometheus.Metrics.CreateCounter("server_total_requests", "Total requests");

//        private readonly JsonSerializerOptions _jsonOptions =
//            new() { PropertyNameCaseInsensitive = true };

//        public MetricsLogger(List<ServerInstance> servers)
//        {
//            _servers = servers;
//            // стартуємо один фон-цикл, який одночасно оновлює метрики і лог
//            var thread = new Thread(async () => await Loop()) { IsBackground = true };
//            thread.Start();
//        }

//        private async Task Loop()
//        {
//            while (true)
//            {
//                await UpdateMetricsFromServers();
//                LogAll();
//                Thread.Sleep(5000);
//            }
//        }

//        private async Task UpdateMetricsFromServers()
//        {
//            foreach (var s in _servers)
//            {
//                try
//                {
//                    var json = await _http.GetStringAsync($"{s.Address}/api/status");
//                    var st = JsonSerializer.Deserialize<ServerState>(json, _jsonOptions);
//                    if (st != null)
//                    {
//                        lock (_lock)
//                        {
//                            CpuUsageGauge.Set(st.CpuUsage);
//                            ActiveRequestsGauge.Set(st.ActiveRequests);
//                            // якщо хочете встановлювати точне значення, а не інкремент:
//                            // TotalRequestsCounter.IncTo(st.TotalRequests);
//                            TotalRequestsCounter.Inc();
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.ForegroundColor = ConsoleColor.Red;
//                    Console.WriteLine($"[MetricsLogger] Помилка при зверненні до {s.Address}: {ex.Message}");
//                    Console.ResetColor();
//                }
//            }
//        }

//        private void LogAll()
//        {
//            lock (_lock)
//            {
//                // Блок 1: Метрики серверів
//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine("=== Server Metrics (every 5s) ===");
//                foreach (var s in _servers)
//                {
//                    Console.WriteLine($"• {s.Id} @ {s.Address}");
//                    Console.WriteLine($"    CPU Usage:       {CpuUsageGauge.Value}%");
//                    Console.WriteLine($"    Active Requests: {ActiveRequestsGauge.Value}");
//                    Console.WriteLine($"    Total Requests:  {TotalRequestsCounter.Value}");
//                }

//                // Блок 2: Логи балансувальника
//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.WriteLine("\n=== Load Balancer Dispatch Counts ===");
//                foreach (var kv in LoadBalancerMiddleware.ServerCounters)
//                {
//                    Console.WriteLine($"Server {kv.Key}: {kv.Value} requests");
//                }

//                Console.ResetColor();
//                Console.WriteLine(); // відступ
//            }
//        }
//    }

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using DistLoad.Models;

namespace DistLoad.Metrics
{
    public class MetricsLogger
    {
        private readonly List<ServerInstance> _servers;
        private readonly HttpClient _http = new();
        private readonly object _lock = new();

        public MetricsLogger(List<ServerInstance> servers)
        {
            _servers = servers;
            var thread = new Thread(Loop) { IsBackground = true };
            thread.Start();
        }

        public void Loop()
        {
            while (true)
            {
                LogAll();
                Thread.Sleep(5000);
            }
        }

        private void LogAll()
        {
            lock (_lock)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("=== Server Metrics (every 5s) ===");

                foreach (var s in _servers)
                {
                    try
                    {
                        var response = _http.GetStringAsync($"{s.Address}/api/status").Result;
                        var state = JsonSerializer.Deserialize<ServerState>(response,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (state != null)
                        {
                            Console.WriteLine($"• {s.Id} @ {s.Address}");
                            Console.WriteLine($"    CPU Usage:       {state.CpuUsage}%");
                            Console.WriteLine($"    Active Requests: {state.ActiveRequests}");
                        }
                        else
                        {
                            Console.WriteLine($"• {s.Id} @ {s.Address}  -- failed to parse");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"• {s.Id} @ {s.Address}  -- error: {ex.Message}");
                    }

                    Console.WriteLine(new string('-', 40));
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n=== Load Balancer Dispatch Counts ===");
                foreach (var kv in LoadBalancerMiddleware.ServerCounters)
                {
                    Console.WriteLine($"Server {kv.Key}: {kv.Value} requests");
                }

                Console.ResetColor();
                Console.WriteLine();
            }
        }
    }
}


