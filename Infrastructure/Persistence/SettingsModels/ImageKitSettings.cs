using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Persistence.SettingsModels;

public class ImageKitSettings
{
    [Required]
    public string? PublicKey { get; set; }
    
    [Required]
    public string? PrivateKey { get; set; }
    
    [Required]
    public string? UrlEndPoint { get; set; }
}