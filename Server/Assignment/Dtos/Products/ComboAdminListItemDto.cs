namespace Assignment.Dtos.Products
{
    public class ComboAdminListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; }
        public int ItemCount { get; set; }
    }
}
