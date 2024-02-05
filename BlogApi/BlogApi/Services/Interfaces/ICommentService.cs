using BlogApi.DTO;

namespace BlogApi.Services.Interfaces;

public interface ICommentService
{
    Task<List<CommentDto>> GetComments(Guid postId);
    Task PostComment(CreateCommentDto createCommentDto, Guid postId, Guid userId);
    Task EditComment(UpdateCommentDto updateCommentDto, Guid commentId, Guid userId);
    Task DeleteComment(Guid commentId, Guid userId);
}