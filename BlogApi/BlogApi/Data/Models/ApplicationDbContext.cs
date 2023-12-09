using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<GroupUser> GroupUsers { get; set; }
    public DbSet<Token> Token { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>().HasKey(x => x.Id);
        modelBuilder.Entity<Post>().HasKey(x => x.Id);
        modelBuilder.Entity<Like>().HasKey(x => x.Id);
        modelBuilder.Entity<User>().HasKey(x => x.Id);
        modelBuilder.Entity<Group>().HasKey(x => x.Id);
        modelBuilder.Entity<Comment>().HasKey(x => x.Id);
        modelBuilder.Entity<PostTag>().HasKey(x => new { x.PostId, x.TagId });
        modelBuilder.Entity<GroupUser>().HasKey(x => new { x.GroupId, x.UserId });
        modelBuilder.Entity<Token>().HasKey(x => x.InvalidToken);
        
        base.OnModelCreating(modelBuilder);
    }
}