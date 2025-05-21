using Prometheus;
using Server1.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Server1.Services
{
    //public class ServerMetricsService
    //{
    //    private readonly ServerState _state = new();
    //    private readonly Random _random = new();

    //    private readonly Gauge _cpuUsage = Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
    //    private readonly Gauge _activeRequests = Metrics.CreateGauge("server_active_requests", "Active Requests");

    //    public ServerMetricsService()
    //    {
    //        //new Timer(_ =>
    //        //{
    //        //    _state.CpuUsage = _random.Next(10, 90);
    //        //    _cpuUsage.Set(*_state.CpuUsage*/);
    //        //}, null, 0, 5000);
    //        new Timer(_ =>
    //        {
    //           var de = _state.CpuUsage = _random.Next(10, 90);
    //            _cpuUsage.Set(de/*_state.CpuUsage*/);
    //        }, null, 0, 5000);

    //    }

    //    public void IncreaseRequests()
    //    {
    //        _state.ActiveRequests++;
    //        _activeRequests.Set(_state.ActiveRequests);
    //    }

    //    public void DecreaseRequests()
    //    {
    //        if (_state.ActiveRequests > 0)
    //            _state.ActiveRequests--;

    //        _activeRequests.Set(_state.ActiveRequests);
    //    }



    //    public ServerState GetServerState()
    //    {
    //        _state.CpuUsage = _cpuUsage.Value;  
    //        _state.ActiveRequests = (int)_activeRequests.Value;  
    //        return _state;
    //    }

    //}


    
        public class ServerMetricsService
        {
            private readonly List<ServerState> _pattern;
            private int _currentIndex;
            private readonly Gauge _cpuUsage = Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
            private readonly Gauge _activeRequests = Metrics.CreateGauge("server_active_requests", "Active Requests");
            private readonly object _lock = new();

            public ServerMetricsService()
            {

                _pattern = new List<ServerState>
            {
                new ServerState { CpuUsage = 10, ActiveRequests =  5 },
                new ServerState { CpuUsage = 20, ActiveRequests = 10 },
                new ServerState { CpuUsage = 50, ActiveRequests = 15 },
                new ServerState { CpuUsage = 80, ActiveRequests =  8 },

            };
                _currentIndex = 0;


                new Timer(_ =>
                {
                    LogNextPattern();
                }, null, 0, 5000);
            }

            private void LogNextPattern()
            {
                ServerState next;
                lock (_lock)
                {
                    next = _pattern[_currentIndex];
                    _currentIndex = (_currentIndex + 1) % _pattern.Count;
                }

                _cpuUsage.Set(next.CpuUsage);
                _activeRequests.Set(next.ActiveRequests);
            }

            public void IncreaseRequests()
            {
                lock (_lock)
                {
                    var v = (int)_activeRequests.Value + 1;
                    _activeRequests.Set(v);
                }
            }

            public void DecreaseRequests()
            {
                lock (_lock)
                {
                    var v = Math.Max(0, (int)_activeRequests.Value - 1);
                    _activeRequests.Set(v);
                }
            }

            public ServerState GetServerState()
            {

                return new ServerState
                {
                    CpuUsage = _cpuUsage.Value,
                    ActiveRequests = (int)_activeRequests.Value,
                    IsAvailable = true
                };
            }
        }
    }

