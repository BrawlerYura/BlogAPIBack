using BlogApi.Data.Models;

namespace BlogApi.Services.Interfaces;

public interface IAddressService
{
    Task<SearchAddressModel> SearchAddress(int parentObjectId, string query);

    Task<SearchAddressModel> ChainAddress(Guid objectGuid);
}