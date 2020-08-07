using AutoMapper;
using Routine.APi.Entities;
using Routine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Profiles
{
    public class CompanyProfiles : Profile
    {
        public CompanyProfiles()
        {
            //        來源      目標
            CreateMap<Company, CompanyDto>()
                .ForMember(
                    dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Name));
            //來源與目標屬性沒有相同時ForMember可以指定dest.CompanyName從src.Name來的屬性

            CreateMap<CompanyAddDto, Company>();
        }
    }
}
