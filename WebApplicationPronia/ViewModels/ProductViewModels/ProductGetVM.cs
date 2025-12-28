namespace WebApplicationPronia.ViewModels.ProductViewModels
{
    public class ProductGetVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public decimal Rating { get; set; }
        public string CategoryName { get; set; }
        public string MainImagePath { get; set; }
        public string HoverImagePath { get; set; }
        public List<string> TagNames { get; set; }
    }
}
