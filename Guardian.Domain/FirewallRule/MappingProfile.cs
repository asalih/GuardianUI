using AutoMapper;

namespace Guardian.Domain.FirewallRule
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Infrastructure.Entity.FirewallRule, FirewallRuleDto>(MemberList.None)
                .ReverseMap();
        }
    }
}
