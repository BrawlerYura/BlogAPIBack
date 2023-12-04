namespace BlogApi.Data.Models;

public class GroupUser
{
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public bool IsAdministrator { get; set; }
}