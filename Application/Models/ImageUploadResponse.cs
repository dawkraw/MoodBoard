using Domain.ValueObjects;

namespace Application.Models;

public record ImageUploadResponse(string ImageUrl, string ImageId, CanvasVector ImageSize);