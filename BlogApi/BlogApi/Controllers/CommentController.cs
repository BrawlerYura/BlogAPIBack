using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BlogApi.Controllers;

[ApiController]
[Route("api")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    [Route("comment/{postId}/tree")]
    [SwaggerOperation(Summary = "Get all nested comments(replies)")]
    public async Task<CommentDto> GetComments(Guid postId)
    {
        return await _commentService.GetComments(postId);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("post/{postId}/comment")]
    [SwaggerOperation(Summary = "Add a comment to a concrete post")]
    public async Task PostComment([FromBody] CreateCommentDto createCommentDto, Guid postId)
    {
        await _commentService.PostComment(createCommentDto, postId);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("comment/{commentId}")]
    [SwaggerOperation(Summary = "Edit concrete comment")]
    public async Task EditComment(UpdateCommentDto updateCommentDto, Guid commentId)
    {
        await _commentService.EditComment(updateCommentDto, commentId);
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("comment/{commentId}")]
    [SwaggerOperation(Summary = "Delete concrete comment")]
    public async Task DeleteComment(Guid commentId)
    {
        await _commentService.DeleteComment(commentId);
    }
}