using DistLoad.Interfaces;
using DistLoad.Models;

namespace DistLoad.Services
{
        public class RoundRobinBalancer : ILoadBalancer
        {
            private readonly List<ServerInstance> _servers;
            private int _lastIndex = -1;

            public RoundRobinBalancer(List<ServerInstance> servers)
            {
                _servers = servers;
            }

            public Task<ServerInstance> GetNextServerAsync()
            {
                if (_servers.Count == 0)
                    throw new Exception("No available servers");

                _lastIndex = (_lastIndex + 1) % _servers.Count;
                return Task.FromResult(_servers[_lastIndex]);
            }
        public List<ServerInstance> GetServers() => _servers;
        }
}
