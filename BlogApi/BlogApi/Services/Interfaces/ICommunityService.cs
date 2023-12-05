using BlogApi.Data.Models;
using BlogApi.DTO;

namespace BlogApi.Services.Interfaces;

public interface ICommunityService
{
    Task<List<CommunityDto>> GetCommunityList();
    
    Task<List<CommunityUserDto>> GetMyCommunityList();

    Task<CommunityFullDto> GetCommunity(Guid communityId);

    Task<PostPagedListDto> GetCommunityPostList(Guid communityId, List<TagDto> tags, PostSorting sorting, int page,
        int size);

    Task<Guid> CreatePost(Guid communityId, CreatePostDto createPostDto);

    Task<CommunityRole?> GetCommunityRole(Guid communityId);

    Task SubscribeToCommunity(Guid communityId);
    
    Task UnsubscribeFromCommunity(Guid communityId);
}