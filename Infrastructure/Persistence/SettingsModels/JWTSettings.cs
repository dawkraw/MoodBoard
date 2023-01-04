using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Persistence.SettingsModels;

public class JWTSettings
{
    [Required]
    public string? SecurityKey { get; set; }
    
    [Required]
    public string? Issuer { get; set; }
    
    [Required]
    public string? Audience { get; set; }
    
    [Required]
    public int TokenExpireTime { get; set; }
}