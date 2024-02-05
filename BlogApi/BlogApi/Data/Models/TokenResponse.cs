using System.ComponentModel.DataAnnotations;

namespace BlogApi.Data.Models;

public class TokenResponse
{
    [Required]
    [MinLength(1)]
    public string Token { get; set; }
}