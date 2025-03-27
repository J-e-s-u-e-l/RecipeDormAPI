using Microsoft.AspNetCore.Http;

namespace RecipeDormAPI.Infrastructure.Infrastructure.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(IFormFile file);
        Task<MemoryStream> GetImageAsync(string filePath);
    }
}
