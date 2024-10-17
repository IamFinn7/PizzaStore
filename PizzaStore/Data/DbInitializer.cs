namespace PizzaStore.Data
{
    public class DbInitializer
    {
        // Initialize data for all tables except Orders and OrderDetails
        public static void Initialize(PizzaContext context)
        {
            // Seed Accounts
            if (!context.Accounts.Any())
            {
                var accounts = new Account[]
                {
                    new Account
                    {
                        UserName = "admin",
                        Password = "1",
                        FullName = "Administrator",
                        Type = AccountType.Staff,
                    },
                    new Account
                    {
                        UserName = "user1",
                        Password = "1",
                        FullName = "John Doe",
                        Type = AccountType.Member,
                    },
                    new Account
                    {
                        UserName = "user2",
                        Password = "password2",
                        FullName = "Jane Smith",
                        Type = AccountType.Member,
                    }
                };
                context.Accounts.AddRange(accounts);
                context.SaveChanges();
            }

            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new Categories[]
                {
                    new Categories { CategoryName = "Pizza", Description = "Delicious pizzas" },
                    new Categories { CategoryName = "Drinks", Description = "Beverages and sodas" },
                    new Categories { CategoryName = "Desserts", Description = "Sweet treats" }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            //// Seed Customers
            //if (!context.Customers.Any())
            //{   
            //    var customers = new Customers[]
            //    {
            //        new Customers
            //        {
            //            ContactName = "Alice Johnson",
            //            Password = "alicepass",
            //            Address = "123 Main St",
            //            Phone = "123-456-7890"
            //        },
            //        new Customers
            //        {
            //            ContactName = "Bob Brown",
            //            Password = "bobpass",
            //            Address = "456 Oak St",
            //            Phone = "987-654-3210"
            //        }
            //    };
            //    context.Customers.AddRange(customers);
            //    context.SaveChanges();
            //}
            // Seed Suppliers
            if (!context.Suppliers.Any())
            {
                var suppliers = new Suppliers[]
                {
                    new Suppliers { CompanyName = "Pizza Supplies Inc.", Address = "789 Food St", Phone = "111-222-3333" },
                    new Suppliers { CompanyName = "Beverages Ltd.", Address = "321 Drink Ave", Phone = "444-555-6666" }
                };
                context.Suppliers.AddRange(suppliers);
                context.SaveChanges();
            }
            // Seed Products
            if (!context.Products.Any())
            {
                var products = new Products[]
                {
                    new Products
                    {
                        ProductName = "Margherita Pizza",
                        SupplierID = 3,
                        CategoryID = 4,
                        QuantityPerUnit = 1,
                        UnitPrice = 9.99m,
                        ProductImage = "https://static01.nyt.com/images/2014/04/09/dining/09JPPIZZA2/09JPPIZZA2-articleLarge-v3.jpg"
                    },
                    new Products
                    {
                        ProductName = "Pepperoni Pizza",
                        SupplierID = 3,
                        CategoryID = 4,
                        QuantityPerUnit = 1,
                        UnitPrice = 11.99m,
                        ProductImage = "https://i0.wp.com/www.amysrecipebook.com/wp-content/uploads/2021/01/pepperonipizza-8-web.jpg?resize=1024%2C683&ssl=1"
                    },
                    new Products
                    {
                        ProductName = "Coca-Cola",
                        SupplierID = 4,
                        CategoryID = 5,
                        QuantityPerUnit = 6,
                        UnitPrice = 4.99m,
                        ProductImage = "https://www.lottemart.vn/media/catalog/product/cache/0x0/8/9/8935049501381-tet-1.jpg.webp"
                    }
                };
                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}
