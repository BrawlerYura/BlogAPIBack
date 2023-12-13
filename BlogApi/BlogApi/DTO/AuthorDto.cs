using System.ComponentModel.DataAnnotations;
using BlogApi.Data.Models;

namespace BlogApi.DTO;

public class AuthorDto
{
    [Required]
    [MinLength(1)]
    public string FullName { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    public int Posts { get; set; }
    public int Likes { get; set; }
    public DateTime CreateTime { get; set; }
}