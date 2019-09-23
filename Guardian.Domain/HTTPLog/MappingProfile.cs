using AutoMapper;

namespace Guardian.Domain.HTTPLog
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Infrastructure.Entity.HTTPLog, HTTPLogDto>(MemberList.None)
                .ReverseMap();
        }
    }
}
