using BlogApi.Data.Models;

namespace BlogApi.DTO;

public class PostPagedListDto
{
    public List<PostDto>? Posts { get; set; }
    public PageInfoModel Pagination { get; set; }
}