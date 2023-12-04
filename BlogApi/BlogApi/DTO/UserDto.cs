using System.ComponentModel.DataAnnotations;
using BlogApi.Data.Models;

namespace BlogApi.DTO;

public class UserDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    [Required]
    [MinLength(1)]
    public string FullName { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
}