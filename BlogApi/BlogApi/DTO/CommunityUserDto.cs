﻿using BlogApi.Data.Models;

namespace BlogApi.DTO;

public class CommunityUserDto
{
    public Guid UserId { get; set; }
    public Guid CommunityId { get; set; }
    public CommunityRole Role { get; set; }
}