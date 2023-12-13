using AutoMapper;
using BlogApi.Data.Models;
using BlogApi.DTO;

namespace DeliveryBackend.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<User, AuthorDto>();
        CreateMap<Group, CommunityDto>();
        CreateMap<Group, CommunityFullDto>();
        CreateMap<Post, PostDto>();
        CreateMap<Post, PostFullDto>();
        CreateMap<Tag, TagDto>();
        CreateMap<Comment, CommentDto>();
    }
}