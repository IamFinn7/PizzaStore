using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Extensions.Options;

namespace PizzaStore.Utility
{
    public class UploadImageService : IUploadImageService
    {
        private readonly FireBaseOptions _fireBaseOptions;
        public UploadImageService(IOptions<FireBaseOptions> fireBaseOptions)
        {
            _fireBaseOptions = fireBaseOptions.Value;
        }

        public async Task<string> UploadImage(IFormFile File)
        {
            var apiKey = _fireBaseOptions.ApiKey;
            var bucket = _fireBaseOptions.Bucket;
            var authEmail = _fireBaseOptions.AuthEmail;
            var authPassword = _fireBaseOptions.AuthPassword;

            var fileName = Path.GetFileName(File.FileName);
            using var stream = new FileStream(Path.Combine(Path.GetTempPath(), fileName), FileMode.Create);
            await File.CopyToAsync(stream);
            stream.Position = 0;

            var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(authEmail, authPassword);

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child("images")
                .Child(fileName)
                .PutAsync(stream, cancellation.Token);
            try
            {
                string link = await task;
                return link;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
    }
}
