using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaStore.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int AccountID { get; set; }
        public Account Account { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string? CartItemsJson { get; set; }
    }
}
