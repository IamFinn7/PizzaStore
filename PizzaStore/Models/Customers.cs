using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaStore.Models
{
    public class Customers
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerID { get; set; }
        public string Password { get; set; }
        public string ContactName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
