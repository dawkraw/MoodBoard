using System.Numerics;
using Application.Interfaces;
using Application.Models;
using Domain.ValueObjects;
using Imagekit;
using Infrastructure.Persistence.SettingsModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence;

public class ImageProcessingService : IImageProcessingService
{
    private readonly ServerImagekit _imageKit;
    private readonly ImageKitSettings _imageKitSettings;

    public ImageProcessingService(IOptions<ImageKitSettings> imageKitSettings)
    {
        _imageKitSettings = imageKitSettings.Value;
        _imageKit = new ServerImagekit(_imageKitSettings.PublicKey, _imageKitSettings.PrivateKey,
            _imageKitSettings.UrlEndPoint);
    }

    public async Task<ImageUploadResponse?> UploadImageByUrlAsync(string url, string imageName)
    {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) return null;
        var uploadResult = await _imageKit.FileName(imageName)
            .UploadAsync(url);

        if (uploadResult.StatusCode != 200) return null;
        if (uploadResult.FileType == "image")
            return new ImageUploadResponse(ImageId: uploadResult.FileId,
                ImageUrl: uploadResult.URL,
                ImageSize: CanvasVector.From(new Vector2(uploadResult.Width, uploadResult.Height)));

        await _imageKit.DeleteFileAsync(uploadResult.FileId);
        return null;
    }

    public async Task<bool> RemoveImageByUrlAsync(string imageId)
    {
        var removeResult = await _imageKit.DeleteFileAsync(imageId);
        return removeResult.StatusCode == 200;
    }
}