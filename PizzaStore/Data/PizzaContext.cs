namespace PizzaStore.Data
{
    public class PizzaContext : DbContext
    {
        public PizzaContext(DbContextOptions<PizzaContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Suppliers> Suppliers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("Account");
            modelBuilder.Entity<Suppliers>().ToTable("Suppliers");
            modelBuilder.Entity<Categories>().ToTable("Categories");
            modelBuilder.Entity<Customers>().ToTable("Customers");
            modelBuilder.Entity<Orders>().ToTable("Orders");
            modelBuilder.Entity<OrderDetails>().ToTable("OrderDetails");
            modelBuilder.Entity<Products>().ToTable("Products");
            modelBuilder.Entity<Cart>().ToTable("Cart");
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.Id);
            });
            modelBuilder.Entity<Account>(entity =>
            {   
                entity.HasKey(a => a.AccountID);
            });
            modelBuilder.Entity<Suppliers>(entity =>
            {
                entity.HasKey(s => s.SupplierID);
            });
            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(c => c.CategoryID);
            });
            modelBuilder.Entity<Customers>(entity =>
            {
                entity.HasKey(c => c.CustomerID);
            });
            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(o => o.OrderID);
            });
            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.HasKey(od => new { od.OrderID, od.ProductID });
            });
            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(p => p.ProductID);
            });

        }
    }
}
