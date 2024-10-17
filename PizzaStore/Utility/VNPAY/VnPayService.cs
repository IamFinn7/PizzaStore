
namespace PizzaStore.Utility.VNPAY
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;
        public VnPayService(IConfiguration config)
        {
            _config =config;
        }
        public string CreatePaymentUrl(HttpContext content, VnPaymentResponseModel model)
        {
           var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount*100).ToString());
            return tick;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            throw new NotImplementedException();
        }
    }
}
