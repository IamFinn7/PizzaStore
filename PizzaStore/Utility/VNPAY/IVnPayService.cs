namespace PizzaStore.Utility.VNPAY
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext content, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
