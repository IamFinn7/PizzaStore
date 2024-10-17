using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PizzaStore.Data;
using PizzaStore.Models;

namespace PizzaStore.Pages.Product
{
    public class IndexModel(PizzaContext context, IMapper mapper) : PageModel
    {
        private readonly PizzaContext _context = context;
        private readonly IMapper _mapper = mapper;

        public IList<ProductVM> Products { get;set; } = default!;
        public string? SearchTerm { get; set; }
        public async Task OnGetAsync(string? searchTerm)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.ProductName.Contains(searchTerm));
            }

            var results = await query.ToListAsync();
            Products = _mapper.Map<List<ProductVM>>(results);
        }
        public async Task<IActionResult> OnPostDemo()
        {
            return Page();
        }
        // Add to cart
        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity)
        {
           
            var accountName = User.Identity.Name;
            if (accountName == null)
            {
                return RedirectToPage("/Login/Login");
            }
            var account = _context.Accounts.FirstOrDefault(u => u.UserName == accountName);
            var accountID = account.AccountID;


            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.Account.UserName == accountName);

            if (cart == null)
            {
                cart = new Cart
                {
                    AccountID = accountID,
                    CartItemsJson = "[]"
                };
                _context.Carts.Add(cart);
            }

            // Check if the ordered quantity exceeds available stock.
            // Throw an exception if the order quantity is greater than the stock quantity.
            if (product.QuantityPerUnit < quantity)
            {
                throw new Exception("Order quantity cannot exceed available stock.");
            }
            // update CartItemsJson with number of items
            var cartItems = JsonConvert.DeserializeObject<List<CartItems>>(cart.CartItemsJson) ?? new List<CartItems>();
            var existingItem = cartItems.FirstOrDefault(ci => ci.ProductID == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                // add new item into cart
                cartItems.Add(new CartItems
                {
                    ProductID = productId,
                    Quantity = quantity,
                    Price = product.UnitPrice,
                    ProductName = product.ProductName
                });
            }
            // Update Quantity Product in stock
            product.QuantityPerUnit -= quantity;
            cart.CartItemsJson = JsonConvert.SerializeObject(cartItems);

            await _context.SaveChangesAsync();
            HttpContext.Session.SetInt32("CartItemCount", cartItems.Count());
            //var age = HttpContext.Session.GetInt32("CartItemCount").ToString();
            return RedirectToPage("Index");
        }
    }
}