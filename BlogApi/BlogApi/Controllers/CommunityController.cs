using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/community")]
public class CommunityController : ControllerBase
{
    private readonly ICommunityService _communityService;

    public CommunityController(ICommunityService communityService)
    {
        _communityService = communityService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get comm list")]
    public async Task<List<CommunityDto>> GetCommunityList()
    {
        return await _communityService.GetCommunityList();
    }
    
    [HttpGet]
    [Route("my")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [SwaggerOperation(Summary = "Get users comm list (with the greatest users role in the comm)")]
    public async Task<List<CommunityUserDto>> GetMyCommunityList()
    {
        return await _communityService.GetMyCommunityList();
    }
    
    [HttpGet]
    [Route("{communityId}")]
    [SwaggerOperation(Summary = "Get info about comm")]
    public async Task<CommunityFullDto> GetCommunity(Guid communityId)
    {
        return await _communityService.GetCommunity(communityId);
    }
    
    [HttpGet]
    [Route("{communityId}/post")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [SwaggerOperation(Summary = "Get comms posts")]
    public async Task<PostPagedListDto> GetCommunityPostList(Guid communityId, List<TagDto> tags, PostSorting sorting, int page = 1,
        int size = 5)
    {
        return await _communityService.GetCommunityPostList(communityId, tags, sorting, page, size);
    }
    
    [HttpPost]
    [Route("{communityId}/post")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [SwaggerOperation(Summary = "Create a post in the spec comm")]
    public async Task<Guid> CreatePost(Guid communityId, CreatePostDto createPostDto)
    {
        return await _communityService.CreatePost(communityId, createPostDto);
    }
    
    [HttpGet]
    [Route("{communityId}/role")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [SwaggerOperation(Summary = "Get the greatest users role in the comm (or null)")]
    public async Task<CommunityRole?> GetCommunityRole(Guid communityId)
    {
        return await _communityService.GetCommunityRole(communityId);
    }
    
    [HttpPost]
    [Route("{communityId}/subscribe")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [SwaggerOperation(Summary = "subscribe to the comm")]
    public async Task SubscribeToCommunity(Guid communityId)
    {
        await _communityService.SubscribeToCommunity(communityId);
    }
    
    [HttpDelete]
    [Route("{communityId}/unsubscribe")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [SwaggerOperation(Summary = "unsubscribe from the comm")]
    public async Task UnsubscribeFromCommunity(Guid communityId)
    {
        await _communityService.UnsubscribeFromCommunity(communityId);
    }
}