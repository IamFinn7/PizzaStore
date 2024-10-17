using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
namespace PizzaStore.Pages.Product
{
    [Authorize(Roles = "Staff")]
    public class CreateModel(PizzaContext context, IMapper mapper, IUploadImageService uploadImageService) : PageModel
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
