using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaStore.Pages.Staff
{
    [Authorize(Roles = "Staff")]
    public class CreateProductModel(PizzaContext context, IMapper mapper, IUploadImageService uploadImageService) : PageModel
    {
        private readonly PizzaContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUploadImageService _uploadImageService = uploadImageService;


        public IActionResult OnGet()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "CompanyName");
            return Page();
        }

        [BindProperty]
        public ProductVM ProductVMs { get; set; } = default!;
        [BindProperty]
        public IFormFile File { get; set; } = default!;


        public async Task<IActionResult> OnPostAsync()
        {
            ProductVMs.ProductImage = await _uploadImageService.UploadImage(this.File);
            _context.Products.Add(_mapper.Map<Products>(ProductVMs));
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
