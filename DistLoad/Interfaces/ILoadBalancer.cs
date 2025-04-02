using DistLoad.Models;

namespace DistLoad.Interfaces
{
    public interface ILoadBalancer
    {
        Task<ServerInstance> GetNextServerAsync();
        List<ServerInstance> GetServers();
    }
}
