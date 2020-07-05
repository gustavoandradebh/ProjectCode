using AutoMapper;
using ProjectCode.Domain.DataTransferObject;
using ProjectCode.Domain.Model;

namespace ProjectCode.Infraestructure.Mapper
{
    public class SdlcSystemMapping: Profile
    {
        public SdlcSystemMapping()
        {
            CreateMap<SdlcSystemModel, Sdlc_System>()
                .ReverseMap(); ;
        }
    }
}
