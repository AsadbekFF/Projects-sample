using AutoMapper;
using ProjectWithAuthenticationSample.ViewModels;
using Sample.Common.Entities;

namespace ProjectWithAuthenticationSample.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserViewModel>().ReverseMap();
        }
    }
}
