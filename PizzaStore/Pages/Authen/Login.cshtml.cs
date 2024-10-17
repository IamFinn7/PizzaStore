using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PizzaStore.Models;

namespace PizzaStore.Pages.Authen
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly PizzaStore.Data.PizzaContext _context;
        public LoginModel(IAuthService authService, PizzaContext context)
        {
            _authService = authService;
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl; 
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl)
        {
            var (isValidUser, role) = await _authService.ValidateUserAsync(Username, Password);
            
            if (isValidUser)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Username),
                new Claim(ClaimTypes.Role, role.ToString())
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                var user = _context.Accounts.FirstOrDefault(u => u.UserName == Username);
                var cart = await _context.Carts
               .FirstOrDefaultAsync(c => c.AccountID == user.AccountID);

                if (cart != null)
                {
                    var cartItems = JsonConvert.DeserializeObject<List<CartItems>>(cart.CartItemsJson) ?? new List<CartItems>();

                    var cartItemCount = cartItems.Count();
                    HttpContext.Session.SetInt32("CartItemCount", cartItemCount);
                }
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToPage("/Index"); 
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
