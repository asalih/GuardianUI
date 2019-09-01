using AutoMapper;

namespace Guardian.Domain.Target
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Infrastructure.Entity.Target, TargetDto>(MemberList.None)
                .ReverseMap();
        }
    }
}
