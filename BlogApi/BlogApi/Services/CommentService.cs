using BlogApi.DTO;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

public class CommentService : ICommentService
{
    public Task<CommentDto> GetComments(Guid postId)
    {
        throw new NotImplementedException();
    }

    public Task PostComment(CreateCommentDto createCommentDto, Guid postId)
    {
        throw new NotImplementedException();
    }

    public Task EditComment(UpdateCommentDto updateCommentDto, Guid commentId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteComment(Guid commentId)
    {
        throw new NotImplementedException();
    }
}