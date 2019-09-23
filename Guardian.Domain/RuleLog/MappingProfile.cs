using AutoMapper;

namespace Guardian.Domain.RuleLog
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Infrastructure.Entity.RuleLog, RuleLogDto>(MemberList.None)
                .ReverseMap();
        }
    }
}
