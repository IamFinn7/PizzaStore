using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Data;
using PizzaStore.Models;

namespace PizzaStore.Pages.Order
{
    public class IndexModel : PageModel
    {
        private readonly PizzaStore.Data.PizzaContext _context;

        public IndexModel(PizzaStore.Data.PizzaContext context)
        {
            _context = context;
        }

        public IList<Orders> Orders { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var isStaff = User.IsInRole("Staff");
            var identityUser = await _context.Accounts.FirstOrDefaultAsync(it => it.UserName == User.Identity.Name);

            if (isStaff)
            {
                Orders = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product) 
                    .ToListAsync();
            }
            else
            {
                Orders = await _context.Orders
                    .Where(u => u.CustomerID == identityUser.AccountID)
                    .Include(o => o.Customer)
                    .Include(o => o.OrderDetails) 
                    .ThenInclude(od => od.Product)
                    .ToListAsync();
            }
        }

    }
}
