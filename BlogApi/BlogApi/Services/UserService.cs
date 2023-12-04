using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

public class UserService : IUsersService
{
    public Task<TokenResponse> RegisterUser(UserRegisterModel userRegisterDto)
    {
        throw new NotImplementedException();
    }

    public Task<TokenResponse> LoginUser(LoginCredentials credentials)
    {
        throw new NotImplementedException();
    }

    public Task Logout(string token)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> GetUserProfile(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task EditUserProfile(Guid userId, UserEditModel userEditModel)
    {
        throw new NotImplementedException();
    }
}