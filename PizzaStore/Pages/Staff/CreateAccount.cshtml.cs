using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaStore.Pages.Staff
{
    public class CreateAccountModel : PageModel
    {
        private readonly PizzaStore.Data.PizzaContext _context;

        public CreateAccountModel(PizzaStore.Data.PizzaContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Account Account { get; set; } = default!;
        public async Task<IActionResult> OnPostAsync()
        {
            //Validate account
            var existUserName = await _context.Accounts.FirstOrDefaultAsync(it => it.UserName == Account.UserName);
            if (existUserName != null)
            {
                ModelState.AddModelError(string.Empty, $"UserName {Account.UserName} already exists.");
                return Page();
            }
            Account accountDto = new Account
            {
                UserName = Account.UserName,
                FullName = Account.FullName,
                Password = Account.Password,
                Type = AccountType.Member
            };
            _context.Accounts.Add(Account);
            await _context.SaveChangesAsync();

            return RedirectToPage("./ManageAccount");
        }
    }
}
