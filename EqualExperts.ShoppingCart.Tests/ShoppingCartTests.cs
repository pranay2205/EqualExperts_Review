using EqualExperts.ShoppingCart.IApiClients;
using EqualExperts.ShoppingCart.Models;
using Moq;
using System.Diagnostics;
using Xunit;

namespace EqualExperts.ShoppingCart.Tests
{
    public class ShoppingCartTest
    {
        private Cart _cart;
        private Mock<IProductApiClient> _mockProductApiClient;
        public ShoppingCartTest()
        {
            _mockProductApiClient = new Mock<IProductApiClient>();
            _cart = new Cart(_mockProductApiClient.Object);
        }
        [Fact]
        public void CreateAShoppingCart_ReturnsTotalItemsInCart()
        {
            Assert.Empty(_cart.CartItems);
        }
        [Fact]
        public async Task AddItemsInShoppingCart_ReturnsCartItems()
        {
            SetMockProductApiClient();
            await _cart.AddItems(new CartItem { ProductName = "Product1", Price = 2.4M, Quantity = 2 });

            Assert.Single( _cart.CartItems);
        }

        [Fact]
        public async Task AddItemsInShoppingCartWithNegativeQuantity_ThrowsException()
        {
            SetMockProductApiClient();
            Assert.ThrowsAsync<ArgumentException>(() => _cart.AddItems(new CartItem { ProductName = "Product1", Price = 2.4M, Quantity = -1 }));
        }
        [Fact]
        public async Task RemoveItemsInShoppingCart_ShouldDecreaseCartSize()
        {
            SetMockProductApiClient();
            var cartItem = new CartItem { ProductName = "Product1", Price = 2.4M, Quantity = 2 };
             await _cart.AddItems(cartItem);

            _cart.RemoveItems("Product1");
            Assert.Empty(_cart.CartItems);    
        }

        [Fact]
        public async Task AddItemsInCart_CallsProductApi()
        {
            _mockProductApiClient
            .Setup(api => api.GetProduct(It.IsAny<string>()))
            .ReturnsAsync(new Product { Title = "Product1", Price = 2.3M }); 
            var cartItem = new CartItem { ProductName = "Product1", Quantity = 2 };
           await _cart.AddItems(cartItem);
            Assert.Equal(2.3M, _cart.CartItems.First().Price);
        }

        [Fact]
        public async Task AddItemsInCart_ProductsApiThrowsError()
        {
            _mockProductApiClient
            .Setup(api => api.GetProduct(It.IsAny<string>()))
            .ThrowsAsync<IProductApiClient, Product>(new Exception());
            var cartItem = new CartItem { ProductName = "Product1", Quantity = 2 };

            Assert.ThrowsAsync<Exception>(async () =>await  _cart.AddItems(cartItem));
        }

        [Fact]
        public async Task AddItems_ChangesCartItemsSubTotal()
        {
            _mockProductApiClient
            .Setup(api => api.GetProduct(It.IsAny<string>()))
            .ReturnsAsync(new Product { Title = "Product1", Price = 2.3M });
            var cartItem = new CartItem { ProductName = "Product1", Quantity = 2 };

            await _cart.AddItems(cartItem);
            _cart.CalculateCartTotal();
            Assert.Equal(4.6M, _cart.SubTotal);
            
        }

        [Fact]
        public async Task AddItems_ChangesCartItemsTax()
        {
            _mockProductApiClient
            .Setup(api => api.GetProduct(It.IsAny<string>()))
            .ReturnsAsync(new Product { Title = "Product1", Price = 2.3M });
            var cartItem = new CartItem { ProductName = "Product1", Quantity = 2 };

            await _cart.AddItems(cartItem);
            _cart.CalculateCartTotal();
            Assert.Equal(0.58m, _cart.Tax);

        }

        [Fact]
        public async Task AddItems_ChangesCartItemsTotal()
        {
            _mockProductApiClient
            .Setup(api => api.GetProduct(It.IsAny<string>()))
            .ReturnsAsync(new Product { Title = "Product1", Price = 2.3M });
            var cartItem = new CartItem { ProductName = "Product1", Quantity = 2 };

            await _cart.AddItems(cartItem);
            _cart.CalculateCartTotal();
            Assert.Equal(5.18m, _cart.Total);

        }
        private void SetMockProductApiClient()
        {
            _mockProductApiClient
                     .Setup(api => api.GetProduct(It.IsAny<string>()))
                     .ReturnsAsync(new Product { Title = "Product1", Price = 2.3M });
        }
    }
}