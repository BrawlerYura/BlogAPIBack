using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

public class PostService : IPostService
{
    public Task<PostPagedListDto> GetPostList(List<TagDto> tags, string author, int min, int max, PostSorting sorting, bool onlyMyCommunities, int page,
        int size)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> CreatePost(CreatePostDto createPostDto)
    {
        throw new NotImplementedException();
    }

    public Task<PostFullDto> GetPost(Guid postId)
    {
        throw new NotImplementedException();
    }

    public Task AddLike(Guid postId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteLike(Guid postId)
    {
        throw new NotImplementedException();
    }
}