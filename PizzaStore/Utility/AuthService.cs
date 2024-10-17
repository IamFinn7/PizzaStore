
namespace PizzaStore.Utility
{
    public class AuthService : IAuthService
    {
        public async Task<(bool IsValid, AccountType? Role)> ValidateUserAsync(string username, string password)
        {
            if (username == "admin" && password == "1")
            {
                return (true, AccountType.Staff);
            }
            else if (username == "user1" && password == "1")
            {
                return (true, AccountType.Member); 
            }
            return (false, null);
        }
    }
}