namespace BlogApi.Data.Models;

public class Tag
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    public List<PostTag> PostTags { get; set; }
}