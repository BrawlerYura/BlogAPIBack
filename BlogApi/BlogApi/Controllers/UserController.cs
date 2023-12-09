using System.Security.Claims;
using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/account/")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Route("register")]
    [SwaggerOperation(Summary = "Register new user")]
    public async Task<TokenResponse> RegisterUser([FromBody] UserRegisterModel userRegisterDto)
    {
        return await _userService.RegisterUser(userRegisterDto);
    }

    [HttpPost]
    [Route("login")]
    [SwaggerOperation(Summary = "Log in to the system")]
    public async Task<TokenResponse> Login([FromBody] LoginCredentials credentials)
    {
        return await _userService.LoginUser(credentials);
    }

    [HttpPost]
    [Route("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [SwaggerOperation(Summary = "Log out system user")]
    public async Task Logout()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
        {
            throw new Exception("Token not found");
        }

        await _userService.Logout(token);
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("profile")]
    [SwaggerOperation(Summary = "Get user profile")]
    public async Task<UserDto> GetUserProfile()
    {
        Guid userId = Guid.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)
            ?.Value);
        
        if (userId == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                "Unauthorized"
            );
            throw ex;
        } else
        {
            return await _userService.GetUserProfile(
                userId
            );
        }
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("profile")]
    [SwaggerOperation(Summary = "Edit user Profile")]
    public async Task EditUserProfile([FromBody] UserEditModel userEditModel)
    {
        await _userService.EditUserProfile(
            Guid.Parse(User.Identity.Name),
            userEditModel
        );
    }
}