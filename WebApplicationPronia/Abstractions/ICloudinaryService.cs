namespace WebApplicationPronia.Abstractions
{
    public interface ICloudinaryService
    {
        Task<string> FileUploadAsync(IFormFile file);
        Task<bool> FileDeleteAsync(string filePath);
    }
}
