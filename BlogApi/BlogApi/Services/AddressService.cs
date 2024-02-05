using AutoMapper;
using BlogApi.Data.Models;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class AddressService : IAddressService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddressService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<SearchAddressModel> SearchAddress(int parentObjectId, string query)
    {
        throw new NotImplementedException();
    }

    public Task<SearchAddressModel> ChainAddress(Guid objectGuid)
    {
        throw new NotImplementedException();
    }
}