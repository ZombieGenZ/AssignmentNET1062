namespace Assignment.Dtos.Admin
{
    public class AdminDashboardDto
    {
        public DashboardSummaryDto Summary { get; set; } = new();
        public List<RevenuePointDto> RevenueSeries { get; set; } = new();
        public List<DashboardOrderDto> RecentOrders { get; set; } = new();
        public List<BestSellerDto> BestSellers { get; set; } = new();
    }
}
