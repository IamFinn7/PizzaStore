using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaStore.Pages.Order
{
    public class OrderModel : PageModel
    {
        private readonly PizzaStore.Data.PizzaContext _context;

        public OrderModel(PizzaStore.Data.PizzaContext context)
        {
            _context = context;
        }

        public IList<Orders> Orders { get; set; } = default!;

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
