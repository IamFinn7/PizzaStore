using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PizzaStore.Data;
using PizzaStore.Models;

namespace PizzaStore.Pages.CartItem
{
    public class IndexModel : PageModel
    {
        private readonly PizzaStore.Data.PizzaContext _context;

        public IndexModel(PizzaStore.Data.PizzaContext context)
        {
            _context = context;
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

        public async Task<IActionResult> OnPostCheckoutAsync(DateTime requiredDate, string address, string phone, string contactName)
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

            return RedirectToPage("/Product/Index");
        }

        public async Task<IActionResult> OnPostPaymentCallBack()
        {
            return RedirectToPage("/Product/Index");
        }
    }
}