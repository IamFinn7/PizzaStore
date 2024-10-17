namespace PizzaStore.Utility
{
    public interface IUploadImageService
    {
        Task<string> UploadImage(IFormFile File);
    }
}
