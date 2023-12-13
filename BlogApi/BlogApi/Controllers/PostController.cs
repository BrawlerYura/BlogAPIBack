using System.Net;
using System.Security.Claims;
using BlogApi.Data;
using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    [Route("")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "ValidateToken")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ExceptionDetails), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Summary = "Get a list of available posts")]
    public async Task<PostPagedListDto> GetPostList(
        [FromQuery] List<Guid>? tags, string? author, int? min, int? max, PostSorting? sorting, bool onlyMyCommunities = false,
        int page = 1, int size = 5
    )
    {
        Guid userId = Guid.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)
            ?.Value ?? string.Empty);
        
        return await _postService.GetPostList(tags, author, min, max, sorting, onlyMyCommunities, page, size, userId);
    }
    
    [HttpPost]
    [Route("")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "ValidateToken")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ExceptionDetails), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Summary = "Create a personal user post")]
    public async Task CreatePost(CreatePostDto createPostDto)
    {
        Guid userId = Guid.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)
            ?.Value ?? string.Empty);
        
        await _postService.CreatePost(createPostDto, userId);
    }
    
    [HttpGet]
    [Route("{postId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "ValidateToken")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType(typeof(ExceptionDetails), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Summary = "Get information about concrete post")]
    public async Task<PostFullDto> GetPost(Guid postId)
    {
        Guid userId = Guid.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)
            ?.Value ?? string.Empty);
        
        return await _postService.GetPost(postId, userId);
    }
    
    [HttpPost]
    [Route("{postId}/like")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "ValidateToken")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType(typeof(ExceptionDetails), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Summary = "Add like to concrete post")]
    public async Task AddLike(Guid postId)
    {
        Guid userId = Guid.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)
            ?.Value ?? string.Empty);
        
        await _postService.AddLike(postId, userId);
    }
    
    [HttpDelete]
    [Route("{postId}/like")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "ValidateToken")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType(typeof(ExceptionDetails), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Summary = "Add like to concrete post")]
    public async Task DeleteLike(Guid postId)
    {
        Guid userId = Guid.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)
            ?.Value ?? string.Empty);
        
        await _postService.DeleteLike(postId, userId);
    }
}