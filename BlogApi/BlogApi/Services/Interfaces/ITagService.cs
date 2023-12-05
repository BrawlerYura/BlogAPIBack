using BlogApi.DTO;

namespace BlogApi.Services.Interfaces;

public interface ITagService
{
    Task<List<TagDto>> GetTagList();
}