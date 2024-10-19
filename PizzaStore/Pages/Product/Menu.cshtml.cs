using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace PizzaStore.Pages.Product
{
    public class MenuModel(PizzaContext context, IMapper mapper) : PageModel
    {
        private readonly PizzaContext _context = context;
        private readonly IMapper _mapper = mapper;
        public IList<ProductVM> Products { get; set; } = default!;

        [BindProperty]
        public ProductVM SelectedProduct { get; set; } = new ProductVM();
        public async Task OnGetAsync(string? searchName, int? categoryId)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(p => p.ProductName.Contains(searchName));
            }
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryID == categoryId.Value);
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            ViewData["SearchName"] = searchName;
            ViewData["SelectedCategoryId"] = categoryId;
            var cateName = await _context.Categories
                            .Where(it => it.CategoryID == categoryId)
                            .Select(it => it.CategoryName)
                            .FirstOrDefaultAsync();
            ViewData["CategoryText"] = cateName;
            var results = await query.ToListAsync();
            Products = _mapper.Map<List<ProductVM>>(results);
        }

        public async Task<IActionResult> OnGetEditAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductID == productId);
            if (product == null)
            {
                return NotFound();
            }

            var productVm = _mapper.Map<ProductVM>(product);
            return new JsonResult(productVm);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var productToUpdate = await _context.Products.FindAsync(SelectedProduct.ProductID);
            if (productToUpdate == null)
            {
                return NotFound();
            }

            productToUpdate.ProductName = SelectedProduct.ProductName;
            productToUpdate.UnitPrice = SelectedProduct.UnitPrice;
            productToUpdate.ProductImage = SelectedProduct.ProductImage;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductsExists(SelectedProduct.ProductID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity)
        {

            var accountName = User.Identity.Name;
            if (accountName == null)
            {
                return RedirectToPage("/Authen/Login");
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
            return RedirectToPage("/Product/Menu");
        }
    }
}
