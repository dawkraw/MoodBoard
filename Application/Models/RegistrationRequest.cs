namespace Application.Models;

public record RegistrationRequest
{
    public string? UserName { get; set; }

    public string? Password { get; set; }
}