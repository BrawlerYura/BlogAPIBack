using System.Net;
using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BlogApi.Controllers;

[ApiController]
[Route("api")]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }
    
    [HttpGet]
    [Route("tag")]
    [ProducesResponseType(typeof(ExceptionDetails), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Summary = "Get tag list")]
    public async Task<List<TagDto>> GetTagList()
    {
        return await _tagService.GetTagList();
    }
}