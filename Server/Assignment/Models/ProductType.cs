namespace Assignment.Models
{
    public class ProductType
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int Sold { get; set; }

        public bool IsPublished { get; set; } = true;
        public bool IsSpicy { get; set; }
        public bool IsVegetarian { get; set; }
        public int SortOrder { get; set; }
    }
}
