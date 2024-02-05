using BlogApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services.Token;

public class TokenCleanupService : BackgroundService
{
    private readonly ILogger<TokenCleanupService> _logger;
    private readonly IServiceProvider _services;

    public TokenCleanupService(ILogger<TokenCleanupService> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                var tokenCleanupTask = scope.ServiceProvider.GetRequiredService<TokenCleanupTask>();
                await tokenCleanupTask.RunAsync(stoppingToken);
            }

            _logger.LogInformation("Token cleanup task completed at: {time}", DateTimeOffset.Now);
            
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}

public class TokenCleanupTask
{
    private readonly ApplicationDbContext _dbContext;

    public TokenCleanupTask(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var expiredTokens = await _dbContext.Token
            .Where(t => t.ExpiredDate < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in expiredTokens)
        {
            _dbContext.Token.Add(token);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}