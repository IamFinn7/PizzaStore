using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaStore.Data;
using PizzaStore.Models;

namespace PizzaStore.Pages.Accounts
{
    [Authorize(Roles = "Staff")]
    public class CreateModel : PageModel
    {
        private readonly PizzaStore.Data.PizzaContext _context;

        public CreateModel(PizzaStore.Data.PizzaContext context)
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

            return RedirectToPage("./Index");
        }
    }
}
