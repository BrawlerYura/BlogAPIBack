using BlogApi.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<Tag> Tag { get; set; }
    public DbSet<Post> Post { get; set; }
    public DbSet<Like> Like { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Group> Group { get; set; }
    public DbSet<Comment> Comment { get; set; }
    public DbSet<PostTag> PostTag { get; set; }
    public DbSet<GroupUser> GroupUser { get; set; }
    public DbSet<Token> Token { get; set; }
    public DbSet<AsAddrObj> AsAddrObj { get; set; }
    public DbSet<AsAdmHierarchy> AsAdmHierarchy { get; set; }
    public DbSet<AsHouse> AsHouse { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>().HasKey(x => x.Id);
        modelBuilder.Entity<Post>().HasKey(x => x.Id);
        modelBuilder.Entity<User>().HasKey(x => x.Id);
        modelBuilder.Entity<Group>().HasKey(x => x.Id);
        modelBuilder.Entity<Comment>().HasKey(x => x.Id);
        modelBuilder.Entity<PostTag>().HasKey(x => new { x.PostId, x.TagId });
        modelBuilder.Entity<GroupUser>().HasKey(x => new { x.GroupId, x.UserId });
        modelBuilder.Entity<Like>().HasKey(x => new { x.UserId, x.PostId });
        modelBuilder.Entity<Token>().HasKey(x => x.InvalidToken);
        modelBuilder.Entity<AsAddrObj>().HasKey(x => x.Id);
        modelBuilder.Entity<AsAdmHierarchy>().HasKey(x => x.Id);
        modelBuilder.Entity<AsHouse>().HasKey(x => x.Id);
        
        base.OnModelCreating(modelBuilder);
    }
}