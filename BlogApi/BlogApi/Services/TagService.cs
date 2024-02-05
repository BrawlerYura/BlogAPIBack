using AutoMapper;
using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class TagService : ITagService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TagService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<List<TagDto>> GetTagList()
    {
        var tags = await _context.Tag.ToListAsync();

        return _mapper.Map<List<TagDto>>(tags);
    }
}