using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Data;
using PizzaStore.Models;

namespace PizzaStore.Pages.Accounts
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly PizzaStore.Data.PizzaContext _context;

        public IndexModel(PizzaStore.Data.PizzaContext context)
        {
            _context = context;
        }

        public IList<Account> Account { get;set; } = default!;

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
    }
}
