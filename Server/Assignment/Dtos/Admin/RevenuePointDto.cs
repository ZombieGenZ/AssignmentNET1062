namespace Assignment.Dtos.Admin
{
    public class RevenuePointDto
    {
        public DateOnly Date { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
        public int OnlineOrders { get; set; }
    }
}
