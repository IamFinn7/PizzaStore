namespace PizzaStore.Helpers
{
    public static class GetPass
    {
        public static string GetMaskedPassword(string password)
        {
            return new string('*', password.Length);
        }

    }
}
