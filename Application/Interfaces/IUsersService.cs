using Application.Models;

namespace Application.Interfaces;

public interface IUsersService
{
    Task<RegistrationResponse> CreateUserAsync(RegistrationRequest registrationRequest);
    Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest authenticationRequest);
    Task<bool> DeleteUserAsync(string userId);
    Task<string> GetUsernameAsync(string userId);
}