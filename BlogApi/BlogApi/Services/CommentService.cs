using AutoMapper;
using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CommentService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<List<CommentDto>> GetComments(Guid postId)
    {
        var comments = await _context.Comments
            .Where(c => c.PostId == postId || c.ParentId == postId)
            .ToListAsync();
        
        return _mapper.Map<List<CommentDto>>(comments);
    }

    public async Task PostComment(CreateCommentDto createCommentDto, Guid postId, Guid userId)
    {
        var userEntity = await _context
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId);
        
        if (userEntity == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                "User not exists"
            );
        }
        
        var newComment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = createCommentDto.Content,
            ParentId = createCommentDto.ParentId,
            PostId = postId,
            UserId = userId
        };
        
        _context.Comments.Add(newComment);
        await _context.SaveChangesAsync();
    }

    public async Task EditComment(UpdateCommentDto updateCommentDto, Guid commentId, Guid userId)
    {
        var comment = await _context
            .Comments
            .FirstOrDefaultAsync(c => c.Id == commentId);
        
        if (comment == null)
        {
            return;
        }

        if (userId != comment.UserId)
        {
            return;
        }
        
        comment.Content = updateCommentDto.Content;
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteComment(Guid commentId, Guid userId)
    {
        var comment = await _context
            .Comments
            .FirstOrDefaultAsync(c => c.Id == commentId);
        
        if (comment == null)
        {
            return;
        }

        if (userId != comment.UserId)
        {
            return;
        }
        
        _context.Comments.Remove(comment);
        
        await _context.SaveChangesAsync();
    }
}