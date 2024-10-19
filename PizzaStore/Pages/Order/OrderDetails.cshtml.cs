using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Data;
using PizzaStore.Models; 

namespace PizzaStore.Pages.Order
{
    public class OrderDetailsModel : PageModel
    {
        private readonly PizzaContext _context;

        public OrderDetailsModel(PizzaContext context)
        {
            _context = context;
        }

        public Orders Order { get; set; } = default!;

        public async Task OnGetAsync(int? id)
        {
            Order = await _context.Orders.Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .FirstOrDefaultAsync(o => o.OrderID == id);
        }
    }
}
