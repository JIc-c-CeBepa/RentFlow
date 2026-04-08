using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
namespace RentFlowApi.Services
{
    public class LocalFileStorageService : IFileStorageInterface
    {
        private readonly IWebHostEnvironment _environment;

        public LocalFileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SavePropertyImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не выбран");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Допустимы только jpg, jpeg, png, webp");

            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var folderPath = Path.Combine(webRootPath, "uploads", "properties");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(folderPath, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/properties/{fileName}";
        }

        public bool DeleteFile(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return false;

            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var cleanPath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(webRootPath, cleanPath);

            if (!File.Exists(fullPath))
                return false;

            File.Delete(fullPath);
            return true;
        }
    }
}
