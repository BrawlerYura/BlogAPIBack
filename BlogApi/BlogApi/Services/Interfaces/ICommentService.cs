using BlogApi.DTO;

namespace BlogApi.Services.Interfaces;

public interface ICommentService
{
    Task<CommentDto> GetComments(Guid postId);
    Task PostComment(CreateCommentDto createCommentDto, Guid postId);
    Task EditComment(UpdateCommentDto updateCommentDto, Guid commentId);
    Task DeleteComment(Guid commentId);
}