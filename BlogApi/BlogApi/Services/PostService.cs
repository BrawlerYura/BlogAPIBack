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

    public async Task<PostPagedListDto> GetPostList(List<Guid>? tags, string? author, int? min, int? max,
        PostSorting? sorting,
        bool onlyMyCommunities, int page, int size, Guid userId)
    {
        var query = _context.Post.AsQueryable();
        
        if (tags != null && tags.Count != 0)
        {
            foreach (var tag in tags)
            {
                var tagDto = await _context.Tag.Where(t => t.Id == tag).FirstOrDefaultAsync();

                if (tagDto == null)
                {
                    throw new BadHttpRequestException($"Tag with id {tag} not found");
                }
            }
            var postIdsWithTag = _context.PostTag
                .Where(pt => tags.Any(t => t == pt.TagId))
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

            query = query.Where(post => post.AuthorId == userIdByAuthor);
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
                query = query.OrderBy(p => p.Likes);
                break;
            case PostSorting.LikeDesc:
                query = query.OrderByDescending(p => p.Likes);
                break;
        }

        int count = (int)Math.Ceiling((double)(query.Count() / size));
        var posts = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var postsFullDto = await CreatePostDto(posts, userId);
        
        var postPagedListDto = new PostPagedListDto
        {
            Posts = postsFullDto,
            Pagination = new PageInfoModel
            {
                Size = postsFullDto.Count,
                Count = count,
                Current = page
            }
        };

        return postPagedListDto;
    }

    public async Task<Guid> CreatePost(CreatePostDto createPostDto, Guid userId)
    {
        foreach (var tagId in createPostDto.Tags)
        {
            var tag = await _context.Tag.Where(x => x.Id == tagId).FirstOrDefaultAsync();
            if (tag == null)
            {
                throw new KeyNotFoundException($"Tag {tagId} not found");
            }
        }

        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = createPostDto.Title,
            Content = createPostDto.Description,
            Description = createPostDto.Description,
            CreateTime = DateTime.UtcNow,
            ReadingTime = createPostDto.ReadingTime,
            Photo = createPostDto.Image,
            AuthorId = userId,
            CommunityId = null,
            AddressId = createPostDto.AddressId,
            Likes = 0
        };

        _context.Post.Add(post);

        await _context.SaveChangesAsync();

        return post.Id;
    }

    public async Task<PostFullDto> GetPost(Guid postId, Guid userId)
    {
        var post = await FindPost(postId);
        
        if(post.CommunityId != null)
        {
            var group = await _context.Group
                .Where(g => g.Id == post.CommunityId)
                .FirstOrDefaultAsync();
            
            var GroupUser = await _context.GroupUser
                .Where(gu => gu.UserId == userId && gu.GroupId == post.CommunityId)
                .FirstOrDefaultAsync();

            if (group.IsClosed && GroupUser == null)
            {
                throw new ForbiddenException("User does not have permissions to view page");
            }
        }
        return await CreatePostFullDto(post, userId, postId);
    }

    public async Task AddLike(Guid postId, Guid userId)
    {
        var post = await FindPost(postId);

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

        post.Likes += 1;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteLike(Guid postId, Guid userId)
    {
        var post = await FindPost(postId);

        var like = await _context.Like
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

        if (like == null)
        {
            throw new ConflictException("There was no like on the post");
        }

        _context.Like.Remove(like);

        post.Likes -= 1;

        await _context.SaveChangesAsync();
    }

    private async Task<Post> FindPost(Guid postId)
    {
        var post = await _context.Post.Where(x => x.Id == postId).FirstOrDefaultAsync();
        if (post == null)
        {
            throw new KeyNotFoundException($"Post with Id {postId} not found");
        }

        return post;
    }

    private async Task<PostFullDto> CreatePostFullDto(Post post, Guid userId, Guid postId)
    {
        var postFullDto = _mapper.Map<PostFullDto>(post);

        var author = await _context.User.Where(u => u.Id == post.AuthorId).FirstOrDefaultAsync();
        if (author == null)
        {
            postFullDto.Author = "-";
        }
        postFullDto.Author = author.FullName;

        if (post.CommunityId != null)
        {
            var group = await _context.Group.Where(g => g.Id == post.CommunityId).FirstOrDefaultAsync();
            postFullDto.CommunityName = group.Name;
        }

        var like = await _context.Like.Where(l => l.UserId == userId && l.PostId == postId).FirstOrDefaultAsync();
        postFullDto.HasLike = (like != null);

        var comments = await _context.Comment.Where(c => c.PostId == postId).ToListAsync();
        postFullDto.CommentsCount = comments.Count;

        postFullDto.Comments = _mapper.Map<List<CommentDto>>(comments);

        var tagDtos = await _context.PostTag
            .Where(pt => pt.PostId == postId)
            .Join(
                _context.Tag,
                pt => pt.TagId,
                tag => tag.Id,
                (pt, tag) => new TagDto
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    CreateTime = tag.CreateTime
                }
            ).ToListAsync();

        postFullDto.Tags = tagDtos;

        return postFullDto;
    }
    
    private async Task<List<PostDto>> CreatePostDto(List<Post> post, Guid userId)
    {
        var postsDto = _mapper.Map<List<PostDto>>(post);

        foreach (var postDto in postsDto)
        {
            var author = await _context.User.Where(u => u.Id == postDto.AuthorId).FirstOrDefaultAsync();
            if (author == null)
            {
                postDto.Author = "-";
            }
            else
            {
                postDto.Author = author.FullName;
            }

            if (postDto.CommunityId != null)
            {
                var group = await _context.Group.Where(g => g.Id == postDto.CommunityId).FirstOrDefaultAsync();
                postDto.CommunityName = group.Name;
            }

            var like = await _context.Like.Where(l => l.UserId == userId && l.PostId == postDto.Id).FirstOrDefaultAsync();
            postDto.HasLike = (like != null);

            var comments = await _context.Comment.Where(c => c.PostId == postDto.Id).ToListAsync();
            postDto.CommentsCount = comments.Count;

            var tagDtos = await _context.PostTag
                .Where(pt => pt.PostId == postDto.Id)
                .Join(
                    _context.Tag,
                    pt => pt.TagId,
                    tag => tag.Id,
                    (pt, tag) => new TagDto
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        CreateTime = tag.CreateTime
                    }
                ).ToListAsync();

            postDto.Tags = tagDtos;
        }
        
        return postsDto;
    }
}