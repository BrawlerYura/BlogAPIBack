namespace BlogApi.Data.Models;

public class Group
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsClosed { get; set; }
    public DateTime CreateTime { get; set; }
    public int SubscribersCount { get; set; } 
}