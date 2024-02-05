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

        var communityFullDto = _mapper.Map<CommunityFullDto>(community);

        var admins = await _context.GroupUser.Where(gu => gu.GroupId == communityId && gu.IsAdministrator == true)
            .Join(
                _context.User,
                gu => gu.UserId,
                u => u.Id,
                (gu, u) => new UserDto()
                {
                    Id = u.Id,
                    CreateTime  = u.CreateTime,
                    FullName = u.FullName,
                    BirthDate = u.BirthDate,
                    Gender = u.Gender,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber
                }
            ).ToListAsync();

        communityFullDto.Administrators = admins;
        
        return communityFullDto;
    }

    public async Task<PostPagedListDto?> GetCommunityPostList(Guid communityId, List<Guid>? tags, PostSorting sorting,
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
        
        if (community.IsClosed && GroupUser == null)
        {
            throw new ForbiddenException("User does not have permissions to view page");
        }
        
        var query = _context.Post
            .Where(p => p.CommunityId == communityId);
        
        if (tags != null && tags.Count != 0)
        {
            var postIdsWithTag = _context.PostTag
                .Where(pt => tags.Any(t => t == pt.TagId))
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

        var postsDto = await CreatePostDto(posts, userId);
        
        var postPagedListDto = new PostPagedListDto
        {
            Posts = postsDto,
            Pagination = new PageInfoModel
            {
                Size = postsDto.Count,
                Count = count,
                Current = page
            }
        };

        return postPagedListDto;
    }

    public async Task<Guid> CreatePost(Guid communityId, CreatePostDto createPostDto, Guid userId)
    {
        foreach (var tagId in createPostDto.Tags)
        {
            var tag = await _context.Tag.Where(x => x.Id == tagId).FirstOrDefaultAsync();
            if (tag == null)
            {
                throw new KeyNotFoundException($"Tag {tagId} not found");
            }
        }

        if (await _context.Post.AnyAsync(p => p.Title == createPostDto.Title))
        {
            throw new ConflictException($"Post with title \"{createPostDto.Title}\" already exists");
        }
        
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = createPostDto.Title,
            Description = createPostDto.Description,
            Content = createPostDto.Content,
            ReadingTime = createPostDto.ReadingTime,
            Photo = createPostDto.Image,
            AddressId = createPostDto.AddressId,
            CreateTime = DateTime.UtcNow,
            AuthorId = userId,
            CommunityId = communityId
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
        var community = await FindCommunity(communityId);
        
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

        community.SubscribersCount += 1;
        
        await _context.SaveChangesAsync();
    }

    public async Task UnsubscribeFromCommunity(Guid communityId, Guid userId)
    {
        var community = await FindCommunity(communityId);
        
        var subscription = await _context.GroupUser
            .FirstOrDefaultAsync(gu => gu.GroupId == communityId && gu.UserId == userId);

        if (subscription == null)
        {
            throw new ConflictException("User already unsubscribed this community");
        }

        _context.GroupUser.Remove(subscription);
        
        community.SubscribersCount -= 1;
        
        await _context.SaveChangesAsync();
    }

    private async Task<Group> FindCommunity(Guid communityId)
    {
        var comm = await _context.Group.Where(x => x.Id == communityId).FirstOrDefaultAsync();
        if (comm == null)
        {
            throw new KeyNotFoundException($"Community with Id {communityId} not found");
        }

        return comm;
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