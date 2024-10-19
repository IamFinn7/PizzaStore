
namespace PizzaStore.Utility
{
    public class AuthService : IAuthService
    {
        private readonly PizzaContext _pizzaContext;
        public AuthService(PizzaContext pizzaContext)
        {
            _pizzaContext = pizzaContext;
        }
        public async Task<(bool IsValid, AccountType? Role)> ValidateUserAsync(string username, string password)
        {
            var identityUser = await _pizzaContext.Accounts.FirstOrDefaultAsync(u => u.Password == password && u.UserName == username);
            if (identityUser != null)
            {
                if(identityUser.Type == AccountType.Staff)
                {
                    return (true, AccountType.Staff);
                }
                else
                {
                    return (true, AccountType.Member);
                }
            }
            return (false, null);
        }
    }
}