namespace WebApplicationPronia.ViewModels.ProductViewModels
{
    public class ProductGetVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Rating { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string MainImagePath { get; set; } = string.Empty;
        public string HoverImagePath { get; set; } = string.Empty;
        public List<string> TagNames { get; set; } = [];
        public List<string> AdditionalImagePaths { get; set; } = [];
    }
}
