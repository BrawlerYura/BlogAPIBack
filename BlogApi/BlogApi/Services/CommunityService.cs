using AutoMapper;
using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Exceptions;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class CommunityService : ICommunityService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CommunityService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CommunityDto>> GetCommunityList()
    {
        var groups = await _context.Group
            .ToListAsync();

        return _mapper.Map<List<CommunityDto>>(groups);
    }

    public async Task<List<CommunityUserDto>> GetMyCommunityList(Guid userId)
    {
        var communityList = await _context.GroupUser
            .Where(gu => gu.UserId == userId)
            .Select(gu => new CommunityUserDto
            {
                UserId = userId,
                CommunityId = gu.GroupId,
                Role = gu.IsAdministrator ? CommunityRole.Administrator : CommunityRole.Subscriber
            })
            .ToListAsync();
        
        return communityList;
    }

    public async Task<CommunityFullDto> GetCommunity(Guid communityId)
    {
        await FindCommunity(communityId);
        
        var community = await _context.Group
            .Where(g => g.Id == communityId)
            .FirstOrDefaultAsync();

        return _mapper.Map<CommunityFullDto>(community);
    }

    public async Task<PostPagedListDto?> GetCommunityPostList(Guid communityId, List<TagDto> tags, PostSorting sorting,
        int page, int size, Guid userId)
    {
        var community = await _context.Group
            .Where(c => c.Id == communityId)
            .FirstOrDefaultAsync();

        if (community == null)
        {
            throw new KeyNotFoundException($"Community with id: {communityId} not found");
        }
        
        var GroupUser = await _context.GroupUser
            .Where(gu => gu.UserId == userId && gu.GroupId == communityId)
            .FirstOrDefaultAsync();

        if (GroupUser == null)
        {
            throw new Exception();
        }
        
        if (community.IsClosed && GroupUser == null)
        {
            throw new ForbiddenException("User does not have permissions to view page");
        }
        
        var query = _context.Post
            .Where(p => p.Id == communityId);

        if (tags != null && tags.Any())
        {
            var postIdsWithTag = _context.PostTag
                .Where(pt => tags.Any(t => t.Id == pt.TagId))
                .Select(pt => pt.PostId)
                .Distinct();
            
            query = query.Where(post => postIdsWithTag.Contains(post.Id));
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

    public async Task<Guid> CreatePost(Guid communityId, CreatePostDto createPostDto)
    {
        // if (createPostDto.Tags == null || !createPostDto.Tags.Any())
        // {
        //     
        // }

        if (await _context.Post.AnyAsync(p => p.Title == createPostDto.Title))
        {
            throw new ConflictException($"Post with title \"{createPostDto.Title}\" already exists");
        }
        
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = createPostDto.Title,
            Description = createPostDto.Description,
            ReadingTime = createPostDto.ReadingTime,
            Photo = createPostDto.Image,
            AddressId = createPostDto.AddressId
        };

        _context.Post.Add(post);

        await _context.SaveChangesAsync();

        var postTags = createPostDto.Tags.Select(tagId => new PostTag
        {
            PostId = post.Id,
            TagId = tagId
        }).ToList();

        _context.PostTag.AddRange(postTags);
        await _context.SaveChangesAsync();

        return post.Id;
    }

    public async Task<CommunityRole?> GetCommunityRole(Guid communityId, Guid userId)
    {
        await FindCommunity(communityId);
        
        var groupUser = await _context.GroupUser
            .Where(gu => gu.UserId == userId && gu.GroupId == communityId)
            .FirstOrDefaultAsync();

        if (groupUser == null)
        {
            throw new Exception();
        }
        
        return groupUser.IsAdministrator ? CommunityRole.Administrator : CommunityRole.Subscriber;
    }

    public async Task SubscribeToCommunity(Guid communityId, Guid userId)
    {
        await FindCommunity(communityId);
        
        var existingSubscription = await _context.GroupUser
            .FirstOrDefaultAsync(gu => gu.GroupId == communityId && gu.UserId == userId);

        if (existingSubscription != null)
        {
            throw new ConflictException("User already subscribed this community");
        }
        
        var newSubscription = new GroupUser
        {
            GroupId = communityId,
            UserId = userId,
            IsAdministrator = false
        };
        
        _context.GroupUser.Add(newSubscription);
        
        await _context.SaveChangesAsync();
    }

    public async Task UnsubscribeFromCommunity(Guid communityId, Guid userId)
    {
        await FindCommunity(communityId);
        
        var subscription = await _context.GroupUser
            .FirstOrDefaultAsync(gu => gu.GroupId == communityId && gu.UserId == userId);

        if (subscription == null)
        {
            throw new ConflictException("User already unsubscribed this community");
        }

        _context.GroupUser.Remove(subscription);
        
        await _context.SaveChangesAsync();
    }

    private async Task FindCommunity(Guid communityId)
    {
        var comm = await _context.Group.Where(x => x.Id == communityId).FirstOrDefaultAsync();
        if (comm == null)
        {
            throw new KeyNotFoundException($"Community with Id {communityId} not found");
        }
    }
}