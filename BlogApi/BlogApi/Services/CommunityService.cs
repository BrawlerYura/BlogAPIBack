using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

public class CommunityService : ICommunityService
{
    public Task<List<CommunityDto>> GetCommunityList()
    {
        throw new NotImplementedException();
    }

    public Task<List<CommunityDto>> GetMyCommunityList()
    {
        throw new NotImplementedException();
    }

    public Task<CommunityFullDto> GetCommunity(Guid communityId)
    {
        throw new NotImplementedException();
    }

    public Task<PostPagedListDto> GetCommunityPostList(Guid communityId, List<TagDto> tags, PostSorting sorting, int page, int size)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> CreatePost(Guid communityId, CreatePostDto createPostDto)
    {
        throw new NotImplementedException();
    }

    public Task<CommunityRole> GetCommunityRole(Guid communityId)
    {
        throw new NotImplementedException();
    }

    public Task SubscribeToCommunity(Guid communityId)
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeFromCommunity(Guid communityId)
    {
        throw new NotImplementedException();
    }
}