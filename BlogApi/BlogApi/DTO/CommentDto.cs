using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTO;

public class CommentDto
{
    [Required]
    public string Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    [Required]
    [MinLength(1)]
    public string Content { get; set; }
    
    [Required]
    public string AuthorId { get; set; }
    
    [Required]
    public int SubComments { get; set; }
}