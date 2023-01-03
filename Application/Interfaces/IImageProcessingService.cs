using Application.Models;

namespace Application.Interfaces;

public interface IImageProcessingService
{
    Task<ImageUploadResponse?> UploadImageByUrlAsync(string url, string imageName);
    Task<bool> RemoveImageByUrlAsync(string imageId);
}