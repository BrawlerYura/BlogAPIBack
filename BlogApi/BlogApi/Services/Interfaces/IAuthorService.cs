using BlogApi.DTO;

namespace BlogApi.Services.Interfaces;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAuthorsList();
}