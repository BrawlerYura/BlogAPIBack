using AutoMapper;
using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Exceptions;
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
        await FindPost(postId);

        var comments = await _context.Comment
            .Where(c => c.PostId == postId || c.ParentId == postId)
            .ToListAsync();

        return _mapper.Map<List<CommentDto>>(comments);
    }

    public async Task PostComment(CreateCommentDto createCommentDto, Guid postId, Guid userId)
    {
        var post = await _context.Post.Where(x => x.Id == postId).FirstOrDefaultAsync();
        if (post == null)
        {
            throw new KeyNotFoundException($"Post with Id {postId} not found");
        }

        if(createCommentDto.ParentId != null)
        {
            var parentComment =
                await _context.Comment.Where(x => x.Id == createCommentDto.ParentId).FirstOrDefaultAsync();
            if (parentComment == null)
            {
                throw new KeyNotFoundException($"Comment with Id {createCommentDto.ParentId} not found");
            }
        }
        
        var community = await _context.Group.Where(x => x.Id == post.CommunityId).FirstOrDefaultAsync();
        var subscription = await _context.GroupUser.Where(x => x.GroupId == community.Id && x.UserId == userId)
            .FirstOrDefaultAsync();

        if (community.IsClosed && subscription != null)
        {
            throw new ForbiddenException("User not subscribed to closed community of post");
        }

        var newComment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = createCommentDto.Content,
            ParentId = createCommentDto.ParentId,
            PostId = postId,
            UserId = userId
        };

        _context.Comment.Add(newComment);
        await _context.SaveChangesAsync();
    }

    public async Task EditComment(UpdateCommentDto updateCommentDto, Guid commentId, Guid userId)
    {
        var comment = await _context.Comment
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
        {
            throw new KeyNotFoundException($"Comment with ID: {commentId} not found");
        }

        if (userId != comment.UserId)
        {
            throw new ForbiddenException($"User cannot edit a comment with ID: {commentId}");
        }

        if (updateCommentDto.Content == comment.Content)
        {
            throw new BadHttpRequestException("Text remained unchanged");
        }

        comment.Content = updateCommentDto.Content;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteComment(Guid commentId, Guid userId)
    {
        var comment = await _context.Comment
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
        {
            throw new KeyNotFoundException($"Comment with ID: {commentId} not found");
        }

        if (userId != comment.UserId)
        {
            throw new ForbiddenException($"User cannot delete a comment with ID: {commentId}");
        }

        _context.Comment.Remove(comment);

        await _context.SaveChangesAsync();
    }

    private async Task FindPost(Guid postId)
    {
        var post = await _context.Post.Where(x => x.Id == postId).FirstOrDefaultAsync();
        if (post == null)
        {
            throw new KeyNotFoundException($"Post with Id {postId} not found");
        }
    }
}