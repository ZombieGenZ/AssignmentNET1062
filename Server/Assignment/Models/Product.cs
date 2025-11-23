namespace Assignment.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();
    }
}
