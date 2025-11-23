namespace Assignment.Dtos.Products
{
    public class ComboListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
