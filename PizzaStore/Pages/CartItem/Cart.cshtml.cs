using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PizzaStore.Utility.VNPAY;

namespace PizzaStore.Pages.CartItem
{
    public class CartModel : PageModel
    {
        private readonly PizzaStore.Data.PizzaContext _context;
        private readonly IVnPayService _vnPayService;
        public CartModel(PizzaStore.Data.PizzaContext context, IVnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
        }

        public List<CartItems> CartItems { get; set; } = new List<CartItems>();
        public decimal TotalPrice { get; set; }
        public async Task OnGetAsync()
        {
            var accountName = User.Identity.Name;

            if (accountName != null)
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(u => u.UserName == accountName);

                if (account != null)
                {
                    var cart = await _context.Carts
                        .FirstOrDefaultAsync(c => c.AccountID == account.AccountID);

                    if (cart != null) 
                    {
                        CartItems = JsonConvert.DeserializeObject<List<CartItems>>(cart.CartItemsJson) ?? new List<CartItems>();
                        TotalPrice = CartItems.Sum(item => item.Price * item.Quantity);
                        //ViewData["CartItemCount"] = CartItemCount;
                    }
                }
            }
        }

        public async Task<IActionResult> OnPostRemoveFromCartAsync(int productId)
        {
            var accountName = User.Identity.Name;
            var exitProduct = await _context.Products.Where(it => it.ProductID == productId).FirstOrDefaultAsync();
            if (accountName != null)
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(u => u.UserName == accountName);

                if (account != null)
                {
                    var cart = await _context.Carts
                        .FirstOrDefaultAsync(c => c.AccountID == account.AccountID);

                    if (cart != null)
                    {
                        var cartItems = JsonConvert.DeserializeObject<List<CartItems>>(cart.CartItemsJson) ?? new List<CartItems>();

                        var itemToRemove = cartItems.FirstOrDefault(item => item.ProductID == productId);
                        if (itemToRemove != null)
                        {
                            cartItems.Remove(itemToRemove);
                            cart.CartItemsJson = JsonConvert.SerializeObject(cartItems);
                            // return quantity if remove
                            exitProduct.QuantityPerUnit += itemToRemove.Quantity;
                            await _context.SaveChangesAsync();
                            HttpContext.Session.SetInt32("CartItemCount", cartItems.Count());
                        }
                    }
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCheckoutAsync(DateTime requiredDate, string address, string phone, string contactName, string paymentType = "COD")
        {
            var accountName = User.Identity.Name;
            // vnpay here
            if (accountName != null)
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(u => u.UserName == accountName);
                if (account != null)
                {
                    var cart = await _context.Carts
                        .FirstOrDefaultAsync(c => c.AccountID == account.AccountID);
                    if (cart != null)
                    {
                        var cartItems = JsonConvert.DeserializeObject<List<CartItems>>(cart.CartItemsJson) ?? new List<CartItems>();
                        decimal freight = cartItems.Sum(it => it.Price) * 0.05m;
                        // Create Customer if new customer
                        var customerExist = await _context.Customers.FindAsync(account.AccountID);
                        Customers cus = null;

                        if (customerExist == null)
                        {
                            cus = new Customers
                            {
                                CustomerID = account.AccountID,
                                Address = address,
                                ContactName = contactName,
                                Password = account.Password,
                                Phone = phone,
                            };

                            await _context.Customers.AddAsync(cus);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            cus = customerExist;
                        }
                        Random random = new Random();
                        var cartTotal = cartItems.Sum(it => it.Price * it.Quantity) * 1000;
                        // VnPay Here
                        if (paymentType == "VNPAY")
                        {
                            var vnPayModel = new VnPaymentRequestModel
                            {
                                Amount = (Double)cartTotal,
                                CreatedDate = DateTime.UtcNow,
                                Description = $"{account.FullName} - {phone}",
                                FullName = contactName,
                                OrderId = random.Next(1000, 100000),
                            };
                            return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
                        }
                        // Add ORDER
                        Orders orders = new Orders
                        {
                            CustomerID = cus.CustomerID,
                            OrderDate = DateTime.Now,
                            RequiredDate = requiredDate,
                            Freight = freight,
                            ShippedDate = requiredDate,
                            ShipAddress = address,
                        };
                        await _context.Orders.AddAsync(orders);
                        await _context.SaveChangesAsync();
                        // add into OrderDetail
                        var orderDetailsList = new List<OrderDetails>();
                        foreach (var item in cartItems)
                        {
                            OrderDetails orderDetails = new OrderDetails
                            {

                                OrderID = orders.OrderID,
                                ProductID = item.ProductID,
                                Quantity = item.Quantity,
                                UnitPrice = item.Price,
                            };
                            orderDetailsList.Add(orderDetails);
                        }
                        // Add range of OrderDetails
                        await _context.OrderDetails.AddRangeAsync(orderDetailsList);
                        cart.CartItemsJson = "[]";
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToPage("./Order/Order");
        }
    }
}
