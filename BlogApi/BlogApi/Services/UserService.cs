using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using BlogApi.Configurations;
using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogApi.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TokenResponse> RegisterUser(UserRegisterModel userRegisterModel)
    {
        userRegisterModel.Email = NormalizeAttribute(userRegisterModel.Email);

        await UniqueCheck(userRegisterModel);
        
        string salt = BCrypt.Net.BCrypt.GenerateSalt();
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterModel.Password, salt);

        CheckGender(userRegisterModel.Gender);
        CheckBirthDate(userRegisterModel.BirthDate);

        await _context.Users.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            FullName = userRegisterModel.FullName,
            BirthDate = userRegisterModel.BirthDate,
            Email = userRegisterModel.Email,
            Gender = userRegisterModel.Gender,
            Password = hashedPassword,
            PhoneNumber = userRegisterModel.PhoneNumber,
        });
        await _context.SaveChangesAsync();

        var credentials = new LoginCredentials
        {
            Email = userRegisterModel.Email,
            Password = userRegisterModel.Password
        };

        return await LoginUser(credentials);
    }

    public async Task<TokenResponse> LoginUser(LoginCredentials credentials)
    {
        credentials.Email = NormalizeAttribute(credentials.Email);

        var identity = await GetIdentity(credentials.Email, credentials.Password);

        var now = DateTime.UtcNow;

        var jwt = new JwtSecurityToken(
            issuer: AuthConfiguration.Issuer,
            audience: AuthConfiguration.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: now.AddMinutes(AuthConfiguration.Lifetime),
            signingCredentials: new SigningCredentials(AuthConfiguration.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        var encodeJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var result = new TokenResponse()
        {
            Token = encodeJwt
        };

        return result;
    }

    public async Task Logout(string token)
    {
        
    }

    public async Task<UserDto> GetUserProfile(Guid userId)
    {
        var userEntity = await _context
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (userEntity != null)
            return _mapper.Map<UserDto>(userEntity);

        var ex = new Exception();
        ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
            "User not exists"
        );
        throw ex;
    }

    public async Task EditUserProfile(Guid userId, UserEditModel userEditModel)
    {
        var userEntity = await _context
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (userEntity == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                "User not exists"
            );
            throw ex;
        }

        CheckGender(userEditModel.Gender);
        CheckBirthDate(userEditModel.BirthDate);

        userEntity.Email = userEditModel.Email;
        userEntity.FullName = userEditModel.FullName;
        userEntity.BirthDate = userEditModel.BirthDate;
        userEntity.Gender = userEditModel.Gender;
        userEntity.PhoneNumber = userEditModel.PhoneNumber;

        await _context.SaveChangesAsync();
    }

    private async Task<ClaimsIdentity> GetIdentity(string email, string password)
    {
        var userEntity = await _context
            .Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (userEntity == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                "User not exists"
            );
            throw ex;
        }

        if (!CheckHashPassword(userEntity.Password, password))
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                "Wrong password"
            );
            throw ex;
        }

        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, userEntity.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity
        (
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType
        );

        return claimsIdentity;
    }

    private static bool CheckHashPassword(string savedPasswordHash, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, savedPasswordHash);
    }

    private static string NormalizeAttribute(string value)
    {
        return value.ToLower().TrimEnd();
    }

    private async Task UniqueCheck(UserRegisterModel userRegisterModel)
    {
        var email = await _context
            .Users
            .Where(x => userRegisterModel.Email == x.Email)
            .FirstOrDefaultAsync();

        if (email != null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                $"Account with email '{userRegisterModel.Email}' already exists"
            );
            throw ex;
        }
    }

    private static void CheckGender(Gender gender)
    {
        if (gender is Gender.Male or Gender.Female) return;

        var ex = new Exception();
        ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
            $"Possible Gender values: {Gender.Male.ToString()}, {Gender.Female.ToString()}");
        throw ex;
    }

    private static void CheckBirthDate(DateTime? birthDate)
    {
        if (birthDate == null || birthDate <= DateTime.Now) return;

        var ex = new Exception();
        ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
            "Birth date can't be later than today");
        throw ex;
    }
}