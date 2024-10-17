namespace PizzaStore.Utility
{
    public interface IAuthService
    {
        Task<(bool IsValid, AccountType? Role)> ValidateUserAsync(string username, string password);
    }
}
