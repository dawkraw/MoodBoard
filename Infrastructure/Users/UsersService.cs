using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence.SettingsModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Users;

public class UsersService : IUsersService
{
    private readonly IMoodBoardDbContext _dbContext;
    private readonly JWTSettings _jwtSettings;
    private readonly IMapper _mapper;
    private readonly UserManager<MoodBoardUser> _userManager;

    public UsersService(UserManager<MoodBoardUser> userManager, IMapper mapper, IOptions<JWTSettings> jwtSettings,
        IMoodBoardDbContext dbContext)
    {
        _userManager = userManager;
        _mapper = mapper;
        _dbContext = dbContext;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<RegistrationResponse> CreateUserAsync(RegistrationRequest registrationRequest)
    {
        var user = _mapper.Map<MoodBoardUser>(registrationRequest);
        var result = await _userManager.CreateAsync(user, registrationRequest.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return new RegistrationResponse {RegistrationSuccessful = false, Errors = errors};
        }

        var mappedUser = _mapper.Map<User>(user);
        _dbContext.MoodBoardUsers.Add(mappedUser);
        await _dbContext.SaveChangesAsync(default);
        return new RegistrationResponse {RegistrationSuccessful = true, UserId = user.Id};
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest authenticationRequest)
    {
        var user = await _userManager.FindByNameAsync(authenticationRequest.UserName);
        var isValidPassword = await _userManager.CheckPasswordAsync(user, authenticationRequest.Password);


        if (user is null || !isValidPassword)
            return new AuthenticationResponse
            {
                AuthenticationSuccessful = false,
                ErrorMessage = "Invalid Credentials"
            };
        
        var token = await GenerateTokenAsync(user);

        return new AuthenticationResponse
        {
            AuthenticationSuccessful = true,
            Token = token,
            UserName = user.UserName,
            IdentityId = user.Id
        };
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var result = await _userManager.DeleteAsync(user);

        if (user is null || !result.Succeeded) return false;

        _dbContext.MoodBoardUsers.Remove(_mapper.Map<User>(user));
        await _dbContext.SaveChangesAsync(default);
        return true;
    }

    public async Task<string> GetUsernameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user.UserName ?? string.Empty;
    }

    private async Task<string> GenerateTokenAsync(MoodBoardUser user)
    {
        var securityKey = Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey);
        var secret = new SymmetricSecurityKey(securityKey);
        var signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

        var userClaims = await _userManager.GetClaimsAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName)
        }.Union(userClaims);

        var tokenOptions = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.Now.AddMilliseconds(_jwtSettings.TokenExpireTime),
            signingCredentials: signingCredentials
        );
        var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return token;
    }
}