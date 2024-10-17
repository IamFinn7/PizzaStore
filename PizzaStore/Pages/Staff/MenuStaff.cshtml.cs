using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaStore.Pages.Staff
{
    public class MenuModel : PageModel
    {
        private readonly PizzaContext _context;
        private readonly IMapper _mapper;

        [BindProperty]
        public List<ProductVM> Products { get; set; } = new List<ProductVM>();

        [BindProperty]
        public ProductVM SelectedProduct { get; set; } = new ProductVM();

        public MenuModel(PizzaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var products = await _context.Products.ToListAsync();
            Products = _mapper.Map<List<ProductVM>>(products);
            return Page();
        }

        public async Task<IActionResult> OnGetEditAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductID == productId);
            if (product == null)
            {
                return NotFound();
            }

            var productVm = _mapper.Map<ProductVM>(product);
            return new JsonResult(productVm);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var productToUpdate = await _context.Products.FindAsync(SelectedProduct.ProductID);
            if (productToUpdate == null)
            {
                return NotFound();
            }

            productToUpdate.ProductName = SelectedProduct.ProductName;
            productToUpdate.UnitPrice = SelectedProduct.UnitPrice;
            productToUpdate.ProductImage = SelectedProduct.ProductImage;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage(); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductsExists(SelectedProduct.ProductID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}
