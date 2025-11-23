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

        // Derived fields
        public bool IsSpicy { get; set; }
        public bool IsVegetarian { get; set; }
        public decimal PriceMin { get; set; }
        public decimal PriceMax { get; set; }
        public int TotalStock { get; set; }
        public int TotalSold { get; set; }
        public Guid? PrimaryProductTypeId { get; set; }
        public ProductType? PrimaryProductType { get; set; }

        public ICollection<ProductType> ProductTypes { get; set; } = new List<ProductType>();
        public ICollection<ProductExtraProduct> ProductExtraProducts { get; set; } = new List<ProductExtraProduct>();

        /// <summary>
        /// Recomputes derived fields based on the current ProductTypes collection.
        /// </summary>
        public void RefreshDerivedFields()
        {
            var publishedTypes = ProductTypes.Where(t => t.IsPublished).ToList();

            IsSpicy = publishedTypes.Any(t => t.IsSpicy);
            IsVegetarian = publishedTypes.Count > 0 && publishedTypes.All(t => t.IsVegetarian);

            if (publishedTypes.Count > 0)
            {
                PriceMin = publishedTypes.Min(t => t.Price);
                PriceMax = publishedTypes.Max(t => t.Price);
                Price = PriceMin;
            }
            else
            {
                PriceMin = PriceMax = Price = 0;
            }

            TotalStock = publishedTypes.Sum(t => t.Stock);
            TotalSold = publishedTypes.Sum(t => t.Sold);

            var primary = publishedTypes
                .OrderBy(t => t.SortOrder)
                .ThenBy(t => t.Price)
                .FirstOrDefault();

            PrimaryProductTypeId = primary?.Id;
            PrimaryProductType = primary;
        }
    }
}
