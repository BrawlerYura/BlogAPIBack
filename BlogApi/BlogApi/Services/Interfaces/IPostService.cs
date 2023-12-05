using BlogApi.Data.Models;
using BlogApi.DTO;

namespace BlogApi.Services.Interfaces;

public interface IPostService
{
    Task<PostPagedListDto> GetPostList(List<TagDto> tags, string author, int min, int max, PostSorting sorting,
        bool onlyMyCommunities, int page, int size);

    Task<Guid> CreatePost(CreatePostDto createPostDto);

    Task<PostFullDto> GetPost(Guid postId);
    
    Task AddLike(Guid postId);
    Task DeleteLike(Guid postId);
}