using EqualExperts.ShoppingCart.Models;
namespace EqualExperts.ShoppingCart.IApiClients
{
    public interface IProductApiClient
    {
        Task<Product> GetProduct(string productName);
    }
}
