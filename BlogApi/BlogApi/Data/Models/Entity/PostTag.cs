namespace BlogApi.Data.Models;

public class PostTag
{
    public Guid PostId { get; set; }
    public Guid TagId { get; set; }
}