using Assignment.Enums;

namespace Assignment.Dtos.Admin
{
    public class DashboardOrderDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
