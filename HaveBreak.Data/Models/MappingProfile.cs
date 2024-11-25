using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HaveBreak.Domain.Accounts;
using HaveBreak.Domain.Posts;

namespace HaveBreak.Data.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserAccount, UserAccountDomain>();
            CreateMap<Post, PostDomain>()
                .ForMember(c=>c.Comments , o=>o.MapFrom(x=>x.Comments.Select(x=>x.Content)))
                .ForMember(c=>c.Likes , o=>o.MapFrom(x=>x.Likes.Count));
        }
    }
}
