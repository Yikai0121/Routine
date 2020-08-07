﻿using AutoMapper;
using Routine.APi.Entities;
using Routine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Routine.Profiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.GenderDisplay, opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest=>dest.Age,opt=>opt.MapFrom(src=>DateTime.Now.Year - src.DateOfBirth.Year));
            CreateMap<EmployeeAddDto, Employee>();
        }
    }
}
