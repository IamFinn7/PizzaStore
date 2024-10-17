using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Data;
using PizzaStore.Models;
using PizzaStore.Utility;

namespace PizzaStore.Pages.CartItem
{
    public class DeleteModel : PageModel
    {
        private readonly PizzaStore.Data.PizzaContext _context;

        public DeleteModel(PizzaStore.Data.PizzaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cart Cart { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FirstOrDefaultAsync(m => m.Id == id);

            if (cart == null)
            {
                return NotFound();
            }
            else
            {
                Cart = cart;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //var allCart = await _context.Carts.CountAsync();
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                Cart = cart;
                _context.Carts.Remove(Cart);
                //HttpContext.Session.SetInt32("CartItemCount", allCart - 1);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
