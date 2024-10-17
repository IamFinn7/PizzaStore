namespace PizzaStore.Models
{
    public class Orders
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public DateTimeOffset RequiredDate { get; set; }
        public DateTimeOffset? ShippedDate { get; set; }
        public decimal Freight { get; set; }
        public string ShipAddress { get; set; }
        public Customers Customer { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}
