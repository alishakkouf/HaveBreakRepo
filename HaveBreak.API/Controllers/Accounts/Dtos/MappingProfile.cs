using AutoMapper;
using HaveBreak.Domain.Accounts;

namespace HaveBreak.API.Controllers.Accounts.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterInputDto, RegisterInputCommand>();
        }
    }
}
