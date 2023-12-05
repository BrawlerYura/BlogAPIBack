using BlogApi.DTO;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

public class TagService : ITagService
{
    public Task<List<TagDto>> GetTagList()
    {
        throw new NotImplementedException();
    }
}