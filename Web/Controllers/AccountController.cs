using Application.Interfaces;
using Application.Models;
using Application.Models.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILoggedInUserService _loggedInUserService;
    private readonly IUsersService _usersService;

    public AccountController(IUsersService usersService, ILoggedInUserService loggedInUserService)
    {
        _usersService = usersService;
        _loggedInUserService = loggedInUserService;
    }

    [HttpPost]
    public async Task<ActionResult> CreateAccount([FromBody] RegistrationRequest registrationRequest)
    {
        var validator = new RegistrationRequestValidator();
        await validator.ValidateAndThrowAsync(registrationRequest);

        var result = await _usersService.CreateUserAsync(registrationRequest);
        if (!result.RegistrationSuccessful) return BadRequest(result.Errors);

        return Created("/", result);
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult> DeleteAccount()
    {
        if (_loggedInUserService.UserId is null) return BadRequest("You are not logged in!");

        var result = await _usersService.DeleteUserAsync(_loggedInUserService.UserId);
        if (!result) return BadRequest("Could not delete your account!");

        return Ok();
    }

    [HttpPost("authenticate")]
    public async Task<ActionResult<AuthenticationResponse>> Authenticate([FromBody] AuthenticationRequest authenticationRequest)
    {
        var validator = new AuthenticationRequestValidator();
        await validator.ValidateAndThrowAsync(authenticationRequest);

        var result = await _usersService.AuthenticateAsync(authenticationRequest);
        if (!result.AuthenticationSuccessful) return BadRequest(result);

        Response.Cookies.Append("mood-session", result.Token!,
            new CookieOptions {HttpOnly = true, SameSite = SameSiteMode.Strict});
        result.Token = null;
        return Ok(result);
    }

    [HttpPost("logout")]
    public ActionResult Logout()
    {
        Response.Cookies.Delete("mood-session");
        return Ok();
    }
}