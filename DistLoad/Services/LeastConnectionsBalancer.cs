using DistLoad.Interfaces;
using DistLoad.Models;

namespace DistLoad.Services
{
        public class LeastConnectionsBalancer : ILoadBalancer
        {
            private readonly List<ServerInstance> _servers;

            public LeastConnectionsBalancer(List<ServerInstance> servers)
            {
                _servers = servers;
            }

            public Task<ServerInstance> GetNextServerAsync()
            {
                if (_servers.Count == 0)
                    throw new Exception("No available servers");

                return Task.FromResult(_servers.OrderBy(s => s.ActiveRequests).First());
            }
        public List<ServerInstance> GetServers() => _servers;
    }

    }
