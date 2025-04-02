namespace Server1.Models
{
        public class ServerState
        {
        public bool IsAvailable { get; set; } = true;
        public int ActiveRequests { get; set; } = 0;
        public double CpuUsage { get; set; } = 0;
        }
    }