using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaStore.Pages.Authen
{
    public class RegisterModel : PageModel
    {
        private readonly PizzaContext _context;

        public RegisterModel(PizzaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Account Account { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existUserName = await _context.Accounts.FirstOrDefaultAsync(it => it.UserName == Account.UserName);
            if (existUserName != null)
            {
                ModelState.AddModelError(string.Empty, $"Username {Account.UserName} already exists.");
                return Page();
            }

            Account accountDto = new Account
            {
                UserName = Account.UserName,
                FullName = Account.FullName,
                Password = Account.Password,
                Type = AccountType.Member,
            };

            _context.Accounts.Add(accountDto);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Authen/Login");
        }

        public async Task<IActionResult> OnGetCheckUserName(string username)
        {
            var exists = await _context.Accounts.AnyAsync(it => it.UserName == username);
            return new JsonResult(!exists);
        }
    }
}
