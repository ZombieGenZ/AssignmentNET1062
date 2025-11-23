namespace Assignment.Dtos.Products
{
    public class ComboDetailDto : ComboListItemDto
    {
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal OriginalPrice { get; set; }
        public List<ComboItemDto> Items { get; set; } = new();
    }
}
