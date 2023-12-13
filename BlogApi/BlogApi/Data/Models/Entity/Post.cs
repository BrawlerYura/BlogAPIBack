namespace BlogApi.Data.Models;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Description { get; set; }
    public DateTime CreateTime { get; set; }
    public int ReadingTime { get; set; }
    public string? Photo { get; set; }
    public Guid AuthorId { get; set; }
    public Guid? CommunityId { get; set; }
    public Guid? AddressId { get; set; }
    public int Likes { get; set; }
}