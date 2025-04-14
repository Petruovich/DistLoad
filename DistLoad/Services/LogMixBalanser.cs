using DistLoad.Interfaces;
using DistLoad.Models;
using System;

namespace DistLoad.Services
{
    public class LogMixBalanser : ILoadBalancer
    {
        private readonly List<ServerInstance> _servers;
        private const double StaticWeight = 100.0;     
        private const double LoadCoefficient = 0.5;    

        public LogMixBalanser(List<ServerInstance> servers)
        {
            _servers = servers;
        }

        public async Task<ServerInstance> GetNextServerAsync()
        {
            ServerInstance bestServer = null;
            double bestScore = double.MinValue;

            foreach (var server in _servers)
            {
                var state = server.LastState;
                if (state == null || !state.IsAvailable)
                    continue;

                double score = CalculateScore(state);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestServer = server;
                }
            }

            return bestServer;
        }

        private double CalculateScore(ServerState state)
        {
            double responseTime = state.ResponseTime > 0 ? state.ResponseTime : 1;
            double activeRequests = state.
                
                
                
                
                
                
                
                
                
                
                
                
                
                ActiveRequests >= 0 ? state.ActiveRequests : 0;

            double denominator = Math.Log(1 + responseTime + LoadCoefficient * activeRequests);
            denominator = denominator > 0 ? denominator : 1; // Захист від ділення на 0

            return StaticWeight / denominator;
        }

        public List<ServerInstance> GetServers() => _servers;
    }
}
