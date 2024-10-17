namespace PizzaStore.Models
{
    public class OrderDetails
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public Orders Order { get; set; }
        public Products Product { get; set; }
    }
}
