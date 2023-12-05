using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTO;

public class UpdateCommentDto
{
    [Required]
    [MaxLength(1000)]
    [MinLength(1)]
    public string Content { get; set; }
}