using System.ComponentModel.DataAnnotations;

namespace BlogApi.Data.Models;

public class Token
{
    [Required]
    public string InvalidToken { get; set; }
    [Required]
    public DateTime ExpiredDate { get; set; }
}