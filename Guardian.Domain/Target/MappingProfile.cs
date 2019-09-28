using AutoMapper;
using System.Linq;

namespace Guardian.Domain.Target
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Infrastructure.Entity.Target, TargetDto>(MemberList.None)
                .ForMember(src => src.ActiveFirewallRulesCount, opt => opt.MapFrom(t => t.FirewallRules.Count(d => d.IsActive)))
                .ForMember(src => src.PassiveFirewallRulesCount, opt => opt.MapFrom(t => t.FirewallRules.Count(d => !d.IsActive)))
                .ReverseMap();
        }
    }
}
