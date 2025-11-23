namespace Assignment.Dtos.Products
{
    public class ComboUpsertDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;
        public List<ComboUpsertItemDto> Items { get; set; } = new();
    }
}
