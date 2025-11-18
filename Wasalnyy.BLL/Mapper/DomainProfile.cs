using AutoMapper;
using Wasalnyy.BLL.DTO.Update;

namespace Wasalnyy.BLL.Mapper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
         CreateMap<UpdateRider,Rider>().ReverseMap();
            CreateMap<UpdateDriver, Driver>().ReverseMap();


        }
    }
}
