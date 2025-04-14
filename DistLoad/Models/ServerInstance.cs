namespace DistLoad.Models
{
    public class ServerInstance
    {
        internal ServerState LastState;

        public string Id { get; set; }
        public string Address { get; set; } 
        public int ActiveRequests { get; set; }
        public bool IsOnline { get; set; } = true;
        public bool IsAvailable => ActiveRequests < 100;
        public int CpuUsage { get; set; } = 0;

    }
}
