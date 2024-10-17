namespace PizzaStore.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        [DisplayFormat(NullDisplayText = "NormalUser")]
        public AccountType? Type { get; set; }
        public Cart Cart { get; set; }
    }
}
