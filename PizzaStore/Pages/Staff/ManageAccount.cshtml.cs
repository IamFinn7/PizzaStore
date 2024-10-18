using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaStore.Pages.Staff
{
    public class ManageAccountModel : PageModel
    {
        private readonly PizzaStore.Data.PizzaContext _context;

        public ManageAccountModel(PizzaStore.Data.PizzaContext context)
        {
            _context = context;
        }

        public IList<Account> Account { get; set; } = default!;

        [BindProperty]
        public Account Accounts { get; set; } = default!;
        public async Task OnGetAsync()
        {
            var accountName = User.Identity.Name;
            var account = await _context.Accounts.SingleOrDefaultAsync(it => it.UserName == accountName);
            bool isStaff = User.IsInRole("Staff");
            if (isStaff)
            {
                Account = await _context.Accounts.ToListAsync();
            }
            else
            {
                Account = await _context.Accounts.Where(it => it.UserName == accountName).ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id) 
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToPage(); 
        }
    }
}
