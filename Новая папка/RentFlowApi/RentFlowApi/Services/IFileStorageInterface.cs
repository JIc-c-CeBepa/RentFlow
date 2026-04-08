namespace RentFlowApi.Services
{
    public interface IFileStorageInterface
    {
        Task<string> SavePropertyImageAsync(IFormFile file);
        bool DeleteFile(string relativePath);
    }
}
