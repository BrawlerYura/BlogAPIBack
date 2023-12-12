using BlogApi.Data.Models;
using BlogApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Services.Interfaces;

public interface IPostService
{
    Task<PostPagedListDto> GetPostList(List<TagDto> tags, string? author, int? min, int? max, PostSorting? sorting,
        bool onlyMyCommunities, int page, int size, Guid userId);

    Task<Guid> CreatePost(CreatePostDto createPostDto, Guid userId);

    Task<PostFullDto> GetPost(Guid postId);
    
    Task AddLike(Guid postId, Guid userId);
    Task DeleteLike(Guid postId, Guid userId);
}