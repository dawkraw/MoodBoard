namespace Application.Models;

public record AuthenticationResponse
{
    public bool AuthenticationSuccessful { get; init; }
    public string? ErrorMessage { get; init; }
    public string? Token { get; set; }

    public string? UserName { get; set; }

    public string? IdentityId { get; set; }
}