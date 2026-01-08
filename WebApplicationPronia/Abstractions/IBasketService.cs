namespace WebApplicationPronia.Abstractions
{
    public interface IBasketService
    {
        Task<List<BasketItem>> GetBasketItemsAsync();
    }
}
