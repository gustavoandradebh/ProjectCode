using AutoMapper;
using ProjectCode.Domain.DataTransferObject;
using ProjectCode.Domain.Model;

namespace ProjectCode.Infraestructure.Mapper
{
    public class ProjectMapping : Profile
    {
        public ProjectMapping()
        {
            CreateMap<ProjectModel, Project>()
                .ForMember(d => d.sdlcSystemId, opt => opt.MapFrom(o => o.sdlcSystem.id))
                .ReverseMap();
        }
    }
}
