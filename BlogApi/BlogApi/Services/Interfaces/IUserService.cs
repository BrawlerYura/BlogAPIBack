using BlogApi.Data.Models;
using BlogApi.DTO;

namespace BlogApi.Services.Interfaces;

public interface IUserService
{
    Task<TokenResponse> RegisterUser(UserRegisterModel userRegisterDto);
    Task<TokenResponse> LoginUser(LoginCredentials credentials);
    Task Logout(string token);
    Task<UserDto> GetUserProfile(Guid userId);
    Task EditUserProfile(Guid userId, UserEditModel userEditModel);
}