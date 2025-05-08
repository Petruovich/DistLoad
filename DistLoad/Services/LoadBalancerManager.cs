using DistLoad.Interfaces;
using DistLoad.Models;

namespace DistLoad.Services
{
    public class LoadBalancerManager : ILoadBalancer
    {
        private readonly List<ServerInstance> _servers;
        private ILoadBalancer _currentBalancer;
        private string _currentAlgorithm;

        public LoadBalancerManager(List<ServerInstance> servers)
        {
            _servers = servers;
            _currentAlgorithm = "roundrobin";
            _currentBalancer = new RoundRobinBalancer(_servers);
        }

        public async Task<ServerInstance> GetNextServerAsync()
        {
            return await _currentBalancer.GetNextServerAsync();
        }

        public List<ServerInstance> GetServers()
        {
            return _servers;
        }

        public string GetCurrentAlgorithm()
        {
            return _currentAlgorithm;
        }

        public bool SetAlgorithm(string algorithm)
        {
            switch (algorithm.ToLower())
            {
                case "roundrobin":
                    _currentBalancer = new RoundRobinBalancer(_servers);
                    _currentAlgorithm = "roundrobin";
                    break;
                case "leastconnections":
                    _currentBalancer = new LeastConnectionsBalancer(_servers);
                    _currentAlgorithm = "leastconnections";
                    break;
                case "adaptive":
                    _currentBalancer = new AdaptiveBalancer(_servers);
                    _currentAlgorithm = "adaptive";
                    break;
                case "logmix":
                    _currentBalancer = new LogMixBalanser(_servers);
                    _currentAlgorithm = "logmix";
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}
