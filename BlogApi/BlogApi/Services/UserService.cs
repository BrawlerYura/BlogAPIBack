using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using AutoMapper;
using BlogApi.Configurations;
using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

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
        userRegisterModel.Password = NormalizeAttribute(userRegisterModel.Password);

        await UniqueCheck(userRegisterModel);
        
        string salt = BCrypt.Net.BCrypt.GenerateSalt();
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterModel.Password, salt);

        CheckGender(userRegisterModel.Gender);
        CheckBirthDate(userRegisterModel.BirthDate);

        await _context.User.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            FullName = userRegisterModel.FullName,
            BirthDate = userRegisterModel.BirthDate,
            Email = userRegisterModel.Email,
            Gender = userRegisterModel.Gender,
            Password = hashedPassword,
            PhoneNumber = userRegisterModel.PhoneNumber,
            CreateTime = DateTime.UtcNow
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
        credentials.Password = NormalizeAttribute(credentials.Password);

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
        var alreadyExistsToken = await _context.Token.FirstOrDefaultAsync(x => x.InvalidToken == token);

        if (alreadyExistsToken == null)
        {
            var handler = new JwtSecurityTokenHandler();
            var expiredDate = handler.ReadJwtToken(token).ValidTo;
            _context.Token.Add(new Data.Models.Token { InvalidToken = token, ExpiredDate = expiredDate });
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new UnauthorizedAccessException("Token is already invalid");
        }
    }

    public async Task<UserDto> GetUserProfile(Guid userId)
    {
        var userEntity = await _context.User
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (userEntity != null)
            return _mapper.Map<UserDto>(userEntity);
        
        throw new UnauthorizedAccessException("User not exists");
    }

    public async Task EditUserProfile(Guid userId, UserEditModel userEditModel)
    {
        var userEntity = await _context.User
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (userEntity == null)
        {
            throw new UnauthorizedAccessException("User not exists");
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
        var userEntity = await _context.User
            .FirstOrDefaultAsync(x => x.Email == email);

        if (userEntity == null)
        {
            throw new BadHttpRequestException("Wrong email address");
        }
        
        if (!CheckHashPassword(userEntity.Password, password))
        {
            throw new BadHttpRequestException("Wrong password");
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

    private static bool CheckHashPassword(string hashedPassword, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
    
    private static string NormalizeAttribute(string value)
    {
        return value.ToLower().TrimEnd();
    }

    private async Task UniqueCheck(UserRegisterModel userRegisterModel)
    {
        var email = await _context.User
            .Where(x => userRegisterModel.Email == x.Email)
            .FirstOrDefaultAsync();

        if (email != null)
        {
            throw new BadHttpRequestException($"Account with email '{userRegisterModel.Email}' already exists");
        }
    }

    private static void CheckPassword(string password)
    {
        if (password.Length < 6)
        {
            throw new BadHttpRequestException("Password must not be less than 6 characters ");
        }
        
        Regex regex = new Regex("^[a-zA-Z0-9!?]*$");

        if (regex.IsMatch(password))
        {
            throw new BadHttpRequestException("Password can contain letters and numbers and ! ?");
        }
        
    }
    
    private static void CheckGender(Gender gender)
    {
        if (gender is Gender.Male or Gender.Female) return;
        
        throw new BadHttpRequestException($"Possible Gender values: {Gender.Male.ToString()}, {Gender.Female.ToString()}");
    }

    private static void CheckBirthDate(DateTime? birthDate)
    {
        if (birthDate == null || birthDate <= DateTime.Now) return;

        throw new BadHttpRequestException("Birth date can't be later than today");
    }
}