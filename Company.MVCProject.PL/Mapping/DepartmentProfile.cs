using AutoMapper;
using Company.MVCProject.DAL.Models;
using Company.MVCProject.PL.Dtos;

namespace Company.MVCProject.PL.Mapping
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<CreateDepartmentDto, Department>().ReverseMap();
        }
    }
}
