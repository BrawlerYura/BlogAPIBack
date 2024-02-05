using AutoMapper;
using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class AuthorService : IAuthorService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AuthorService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<List<AuthorDto>> GetAuthorsList()
    {
        var users = await _context.User.ToListAsync();
        var authorsDto = _mapper.Map<List<AuthorDto>>(users);

        int i = 0;
        foreach(var user in users) 
        {
            var likes = await _context.Like.Where(l => l.UserId == user.Id).ToListAsync();
            var posts = await _context.Post.Where(p => p.AuthorId == user.Id).ToListAsync();
            
            authorsDto[i].Likes = likes.Count;
            authorsDto[i].Posts = posts.Count;
            i++;
        }
        
        return authorsDto;
    }
}