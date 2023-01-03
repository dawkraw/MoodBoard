namespace Application.Models;

public record RegistrationResponse
{
    public bool RegistrationSuccessful { get; init; }

    public string? UserId { get; init; }
    public IEnumerable<string>? Errors { get; init; }
}