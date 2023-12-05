using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/post")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get a list of available posts")]
    public async Task<PostPagedListDto> GetPostList(
        List<TagDto> tags, string author, int min, int max, PostSorting sorting, bool onlyMyCommunities = false,
        int page = 1, int size = 5
    )
    {
        return await _postService.GetPostList(tags, author, min, max, sorting, onlyMyCommunities, page, size);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Create a personal user post")]
    public async Task CreatePost(CreatePostDto createPostDto)
    {
        await _postService.CreatePost(createPostDto);
    }
    
    [HttpGet]
    [Route("{postId}")]
    [SwaggerOperation(Summary = "Get information about concrete post")]
    public async Task<PostFullDto> GetPost(Guid postId)
    {
        return await _postService.GetPost(postId);
    }
    
    [HttpPost]
    [Route("{postId}/like")]
    [SwaggerOperation(Summary = "Add like to concrete post")]
    public async Task AddLike(Guid postId)
    {
        await _postService.AddLike(postId);
    }
    
    [HttpDelete]
    [Route("{postId}/like")]
    [SwaggerOperation(Summary = "Add like to concrete post")]
    public async Task DeleteLike(Guid postId)
    {
        await _postService.DeleteLike(postId);
    }
}