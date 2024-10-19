using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using PizzaStore.Utility;
using System.Configuration;

namespace PizzaStore
{   
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddPizzaStoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PizzaContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PizzaStore")));
            services.AddTransient(typeof(IUploadImageService), typeof(UploadImageService));
            services.AddScoped<IAuthService, AuthService>();
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.Configure<FireBaseOptions>(configuration.GetSection("FireBase"));
            services.AddHttpContextAccessor();
            services.AddScoped<ShoppingCartService>();
            return services;
        }
    }
}
