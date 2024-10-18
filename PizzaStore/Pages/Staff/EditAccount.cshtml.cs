using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaStore.Pages.Staff
{
    public class EditAccountModel : PageModel
    {
        private readonly PizzaStore.Data.PizzaContext _context;

        public EditAccountModel(PizzaStore.Data.PizzaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Account Account { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.AccountID == id);
            if (account == null)
            {
                return NotFound();
            }
            Account = account;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _context.Attach(Account).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(Account.AccountID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToPage("./ManageAccount");
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountID == id);
        }
    }
}
