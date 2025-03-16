using EqualExperts.ShoppingCart.IApiClients;


namespace EqualExperts.ShoppingCart.Models
{
    public class Cart
    {
        public List<CartItem> CartItems { get; set; }
        public IProductApiClient _productApiClient;
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public const decimal TaxPercentage = 0.125M;
        public Cart(IProductApiClient productApiClient)
        {
            CartItems = new List<CartItem>();  
            _productApiClient = productApiClient;

        }

        public async Task AddItems(CartItem item)
        {
            if (item.Quantity <= 0)
            {
                throw new ArgumentException();
            }
            var product = await _productApiClient.GetProduct(item.ProductName);
            if (product == null)
            {
                throw new ArgumentException("Invalid product name");
            }
            item.Price = product.Price;
            CartItems.Add(item);
        }

        public void CalculateCartTotal()
        {
            foreach(var item in CartItems)
            {
                SubTotal += item.Price * item.Quantity;
            }
            Tax = Math.Round(SubTotal * TaxPercentage, 2);
            Total = SubTotal + Tax;
        }

        public List<CartItem> GetItems()
        {
            return CartItems;
        }
        public void RemoveItems(string productName)
        {
           
            var existingItem = CartItems.FirstOrDefault(x=>x.ProductName==productName);
            if (existingItem == null)
                throw new ArgumentException("Item not found in cart");
            
            CartItems.Remove(existingItem);
        }
    }
}
