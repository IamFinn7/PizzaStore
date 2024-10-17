namespace PizzaStore.Utility.VNPAY
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext content, VnPaymentResponseModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
