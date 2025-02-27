namespace API.Models
{
    public class DashboardStatsDTO
    {
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int BlockedUsers { get; set; }
        public int FailedLogins { get; set; }
    }
}
