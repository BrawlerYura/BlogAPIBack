using System.Net;
using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/author/list")]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(ExceptionDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<List<AuthorDto>> GetAuthorsList()
    {
        return await _authorService.GetAuthorsList();
    }
}