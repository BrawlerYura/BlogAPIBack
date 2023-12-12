using AutoMapper;
using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Exceptions;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class PostService : IPostService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PostService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<PostPagedListDto> GetPostList([FromQuery]List<TagDto> tags, string? author, int? min, int? max, PostSorting? sorting,
        bool onlyMyCommunities, int page, int size, Guid userId)
    {
    
        var query = _context.Post.AsQueryable();
        
        if (tags != null && tags.Any())
        {
            var postIdsWithTag = _context.PostTag
                .Where(pt => tags.Any(t => t.Id == pt.TagId))
                .Select(pt => pt.PostId)
                .Distinct();
            
            query = query.Where(post => postIdsWithTag.Contains(post.Id));
        }
        
        if (!string.IsNullOrEmpty(author))
        {
            var userIdByAuthor = await _context.User
                .Where(u => u.FullName == author)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();
            
            query = query.Where(post => post.UserId == userIdByAuthor);
        }

        if (min != null)
        {
            query = query.Where(post => post.ReadingTime >= min);
        }

        if (max != null)
        {
            query = query.Where(post => post.ReadingTime <= max);
        }

        if (onlyMyCommunities)
        {
            query = query.Where(post => _context.GroupUser
                .Any(gu => gu.UserId == userId && gu.GroupId == post.CommunityId));
        }
        
        switch (sorting)
        {
            case PostSorting.CreateDesc:
                query = query.OrderByDescending(p => p.CreateTime);
                break;
            case PostSorting.CreateAsc:
                query = query.OrderBy(p => p.CreateTime);
                break;
            case PostSorting.LikeAsc:
                query = query.OrderBy(p => p.Likes.Count);
                break;
            case PostSorting.LikeDesc:
                query = query.OrderByDescending(p => p.Likes.Count);
                break;
        }
        
        var posts = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
        
        var postPagedListDto = new PostPagedListDto
        {
            Posts = _mapper.Map<List<PostDto>>(posts),
            Pagination = new PageInfoModel
            {
                Size = size,
                Count = size,
                Current = page
            }
        };

        return postPagedListDto;
    }

    public async Task<Guid> CreatePost(CreatePostDto createPostDto, Guid userId)
    {
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = createPostDto.Title,
            Content = createPostDto.Description,
            Description = createPostDto.Description,
            CreateTime = DateTime.Now,
            ReadingTime = createPostDto.ReadingTime,
            Photo = createPostDto.Image,
            UserId = userId,
            CommunityId = null,
            AddressId = createPostDto.AddressId
        };
        
        _context.Post.Add(post);
        
        await _context.SaveChangesAsync();
        
        return post.Id;
    }

    public async Task<PostFullDto> GetPost(Guid postId)
    {
        await FindPost(postId);
        
        var post = await _context.Post
            .Where(p => p.Id == postId)
            .FirstOrDefaultAsync();

        return _mapper.Map<PostFullDto>(post);
    }

    public async Task AddLike(Guid postId, Guid userId)
    {
        await FindPost(postId);
        
        var existingLike = await _context.Like
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

        if (existingLike != null)
        {
            throw new ConflictException("Like already exists");
        }
        
        var newLike = new Like
        {
            UserId = userId,
            PostId = postId
        };
        
        _context.Like.Add(newLike);
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLike(Guid postId, Guid userId)
    {
        await FindPost(postId);
        
        var like = await _context.Like
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

        if (like == null)
        {
            throw new ConflictException("There was no like on the post");
        }

        _context.Like.Remove(like);
        
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