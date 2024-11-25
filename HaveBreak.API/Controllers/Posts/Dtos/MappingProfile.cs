using AutoMapper;
using HaveBreak.Domain.Posts;

namespace HaveBreak.API.Controllers.Posts.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PostDomain, PostDto>();
            CreateMap<GetPostsFeedRequestDto, GetPostsFeedRequestQuery>();
        }
    }
}
