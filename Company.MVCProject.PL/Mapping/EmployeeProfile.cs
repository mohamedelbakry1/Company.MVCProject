using AutoMapper;
using Company.MVCProject.DAL.Models;
using Company.MVCProject.PL.Dtos;

namespace Company.MVCProject.PL.Mapping
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            //CreateMap<CreateEmployeeDto, Employee>()
            //    .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));
            //CreateMap<Employee, CreateEmployeeDto>().ReverseMap();

            CreateMap<Employee, CreateEmployeeDto>();
            CreateMap<CreateEmployeeDto, Employee>();

        }
    }
}
