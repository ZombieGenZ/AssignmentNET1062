namespace Assignment.Dtos.Admin
{
    public class BestSellerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Sold { get; set; }
        public decimal Revenue { get; set; }
    }
}
