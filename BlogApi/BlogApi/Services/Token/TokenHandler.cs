using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using BlogApi.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace BlogApi.Services.Token;

public class ValidateTokenRequirement : IAuthorizationRequirement
{
    public ValidateTokenRequirement()
    {
    }
}

public class ValidateTokenHandler : AuthorizationHandler<ValidateTokenRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ValidateTokenHandler(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory serviceScopeFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidateTokenRequirement requirement)
    {
        try
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                string? authorizationString = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];
                if (string.IsNullOrEmpty(authorizationString) || !authorizationString.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException("Invalid token");
                }

                var token = authorizationString.Substring("Bearer ".Length).Trim();

                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var tokenEntity = await dbContext.Token
                    .FirstOrDefaultAsync(x => x.InvalidToken == token);

                if (tokenEntity != null && tokenEntity.ExpiredDate > DateTime.UtcNow)
                {
                    throw new UnauthorizedAccessException("Token expired");
                }
            }

            context.Succeed(requirement);
        }
        catch (UnauthorizedAccessException ex)
        {
            var response = _httpContextAccessor.HttpContext?.Response;
            if (response != null)
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.ContentType = "application/json";

                ProblemDetails problem = new()
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Type = "Unauthorized",
                    Title = "Unauthorized",
                    Detail = ex.Message
                };

                string json = JsonSerializer.Serialize(problem);
                
                await response.WriteAsync(json);
            }
        }
    }
}

