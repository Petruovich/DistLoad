namespace DistLoad.Models
{
    public class ServerState
    {
        //internal int ResponseTime;

        public bool IsAvailable { get; set; } = true;
        public int ActiveRequests { get; set; } = 0;
        public double CpuUsage { get; set; } = 0;
        public int ResponseTime { get; internal set; }
    }
}
