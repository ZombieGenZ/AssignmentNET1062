namespace Assignment.Dtos.Admin
{
    public class DashboardSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AvgOrderValue { get; set; }
        public int OnlinePaymentRate { get; set; }
        public int RevenueGrowth { get; set; }
        public int FulfillmentRate { get; set; }
    }
}
