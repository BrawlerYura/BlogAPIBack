using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTO;

public class CreateCommentDto
{
    [Required]
    [MaxLength(1000)]
    [MinLength(1)]
    public string Content { get; set; }
    
    public Guid? Parentid { get; set; }
}