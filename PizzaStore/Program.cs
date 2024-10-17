using Microsoft.AspNetCore.Authentication.Cookies;

namespace PizzaStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services
                    .AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    })
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Login/Login";
                        options.AccessDeniedPath = "/AccessDenied";
                        options.ReturnUrlParameter = "ReturnUrl";
                    }
                    //.AddFacebook(options =>
                    //{
                    //    options.AppId = "YOUR_FACEBOOK_APP_ID";
                    //    options.AppSecret = "YOUR_FACEBOOK_APP_SECRET";
                    //    options.SaveTokens = true;
                    //})
                    //.AddGoogle(options =>
                    //{
                    //    options.ClientId = "YOUR_GOOGLE_CLIENT_ID";
                    //    options.ClientSecret = "YOUR_GOOGLE_CLIENT_SECRET";
                    //    options.SaveTokens = true;
                    //});
                    );
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));
                options.AddPolicy("MemberOnly", policy => policy.RequireRole("Member"));
            });
            // Add services to the container.
            builder.Services.AddRazorPages();
         
            builder.Services.AddPizzaStoreServices(builder.Configuration);
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            CreateDbIfNotExists(app);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.MapRazorPages();

            app.Run();
        }
        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<PizzaContext>();
                    context.Database.EnsureCreated();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
    }
}
