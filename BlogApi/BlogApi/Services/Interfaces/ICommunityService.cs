using BlogApi.Data.Models;
using BlogApi.DTO;

namespace BlogApi.Services.Interfaces;

public interface ICommunityService
{
    Task<List<CommunityDto>> GetCommunityList();
    
    Task<List<CommunityUserDto>> GetMyCommunityList(Guid userId);

    Task<CommunityFullDto> GetCommunity(Guid communityId);

    Task<PostPagedListDto?> GetCommunityPostList(Guid communityId, List<Guid>? tags, PostSorting sorting, int page,
        int size, Guid userId);

    Task<Guid> CreatePost(Guid communityId, CreatePostDto createPostDto, Guid userId);

    Task<CommunityRole?> GetCommunityRole(Guid communityId, Guid userId);

    Task SubscribeToCommunity(Guid communityId, Guid userId);
    
    Task UnsubscribeFromCommunity(Guid communityId, Guid userId);
}