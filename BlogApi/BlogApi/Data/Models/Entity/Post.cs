namespace BlogApi.Data.Models;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int ReadingTime { get; set; }
    public string Photo { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public List<Like> Likes { get; set; }
    public List<Comment> Comments { get; set; }
    public List<PostTag> PostTags { get; set; }
}