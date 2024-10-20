using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> OnPostAsync()
        {
            var account = await _context.Accounts.AsNoTracking().SingleOrDefaultAsync(it => it.UserName == User.Identity.Name);
            var adminAccount = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(it => it.Type == AccountType.Staff);
            var isStaff = User.IsInRole("Staff");
            if (adminAccount.AccountID == Account.AccountID && isStaff)
            {
                Account.Type = AccountType.Staff;
            }
            else if (account.Type == AccountType.Member)
            {
                Account.Type = AccountType.Member;
            }
            else if(account.Type == null)
            {
                Account.Type = null;
            }
            else
            {
                Account.Type = null;
            }
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
