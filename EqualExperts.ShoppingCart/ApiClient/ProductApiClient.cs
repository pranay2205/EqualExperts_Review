using EqualExperts.ShoppingCart.IApiClients;
using EqualExperts.ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EqualExperts.ShoppingCart.ApiClient
{
    public class ProductApiClient : IProductApiClient
    {
        private HttpClient _httpClient;
        private string baseUrl = "https://equalexperts.github.io/";
        public ProductApiClient() { 
            _httpClient = new HttpClient();
        }

        public async Task<Product> GetProduct(string productName)
        {
            
            try
            {
                var url = $"/backend-take-home-test-data/{productName}.json";
                var result = await _httpClient.GetAsync(url);
                if(result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                   return JsonSerializer.Deserialize<Product>(content);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add product to cart: " + ex.Message, ex);
            }
            return null;
        }
    }
}
