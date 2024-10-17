namespace PizzaStore.Models
{
    public class Categories
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
