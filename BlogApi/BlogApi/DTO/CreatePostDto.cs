﻿using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTO;

public class CreatePostDto
{
    [Required]
    [MaxLength(1000)]
    [MinLength(5)]
    public string Title { get; set; }
    
    [Required]
    [MaxLength(5000)]
    [MinLength(5)]
    public string Description { get; set; }
    
    [Required]
    [MaxLength(1000)]
    [MinLength(1)]
    public string Content { get; set; }
    
    [Required]
    public int ReadingTime { get; set; }
    
    [Url]
    [MaxLength(1000)]
    public string? Image { get; set; }
    
    public Guid? AddressId { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<Guid> Tags { get; set; }
}