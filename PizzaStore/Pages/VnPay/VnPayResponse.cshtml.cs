using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PizzaStore.Utility.VNPAY;

namespace PizzaStore.Pages.VnPay
{
    public class VnPayResponseModel : PageModel
    {
        private readonly IVnPayService _vnPayService;
        public VnPayResponseModel(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }
        public string PaymentMessage { get; set; }
        public bool IsSuccess { get; set; } = false;
        public async Task<IActionResult> OnGetAsync()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Error when payment: {response?.VnPayResponseCode ?? "Unknown"}";
                return RedirectToPage("/Product/Index");
            }
            else
            {
                PaymentMessage = $"Success Payment: {response.TransactionId}";
                IsSuccess = true;
                // Store order into database with VNPAY payment type
                // Implement here
                return RedirectToPage("/Product/Menu");
            }
        }
    }
}