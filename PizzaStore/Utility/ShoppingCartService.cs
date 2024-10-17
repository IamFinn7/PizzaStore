using Newtonsoft.Json;

namespace PizzaStore.Utility
{
    public class ShoppingCartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartCookieName = "ShoppingCart";

        public ShoppingCartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CartItem> GetCartItems()
        {
            var cartCookie = _httpContextAccessor.HttpContext.Request.Cookies[CartCookieName];
            if (string.IsNullOrEmpty(cartCookie))
            {
                return new List<CartItem>();
            }

            return JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);
        }

        public void AddToCart(CartItem item)
        {
            var cartItems = GetCartItems();
            var existingItem = cartItems.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cartItems.Add(item);
            }

            var cartJson = JsonConvert.SerializeObject(cartItems);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(CartCookieName, cartJson);
        }

        public void ClearCart()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(CartCookieName);
        }
    }
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Quantity * Price;
    }

}
