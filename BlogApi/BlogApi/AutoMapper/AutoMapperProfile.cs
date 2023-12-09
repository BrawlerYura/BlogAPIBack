using AutoMapper;
using BlogApi.Data.Models;
using BlogApi.DTO;

namespace DeliveryBackend.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>();
    }
}