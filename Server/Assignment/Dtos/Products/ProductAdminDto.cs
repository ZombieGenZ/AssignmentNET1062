namespace Assignment.Dtos.Products
{
    public class ProductAdminDto : ProductDetailDto
    {
        public string? CategoryName { get; set; }
        public bool IsActive { get; set; }
    }
}
