using AutoMapper;
using static Guardian.Domain.Account.SignUp;

namespace Guardian.Domain.Account
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AccountDto, Infrastructure.Entity.Account>()
                .ForMember(dest => dest.Targets, act => act.Ignore());

            CreateMap<Infrastructure.Entity.Account, AccountDto>()
                .ForMember(dest => dest.Password, act=> act.Ignore())
                .ForMember(dest => dest.Salt, act => act.Ignore());

            CreateMap<AccountSignUpDto, Infrastructure.Entity.Account>()
                .ReverseMap();
        }
    }
}
