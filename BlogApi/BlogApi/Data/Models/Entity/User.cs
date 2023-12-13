using System.ComponentModel.DataAnnotations;

namespace BlogApi.Data.Models;

public class User
{
    public Guid Id { get; set; }
    [Required]
    [MinLength(1)]
    public string FullName { get; set; }
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    
    public DateTime CreateTime { get; set; }
    
    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
}