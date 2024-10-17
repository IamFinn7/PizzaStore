namespace PizzaStore.Models
{
    public class Suppliers
    {
        public int SupplierID { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
