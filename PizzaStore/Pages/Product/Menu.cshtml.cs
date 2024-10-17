using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PizzaStore.Pages.Product
{
    public class MenuModel(PizzaContext context, IMapper mapper) : PageModel
    {
        private readonly PizzaContext _context = context;
        private readonly IMapper _mapper = mapper;
        public IList<ProductVM> Products { get; set; } = default!;
        public async Task OnGetAsync(string? searchTerm)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.ProductName.Contains(searchTerm));
            }

            var results = await query.ToListAsync();
            Products = _mapper.Map<List<ProductVM>>(results);
        }
    }
}
