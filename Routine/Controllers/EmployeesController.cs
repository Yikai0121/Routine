using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Routine.APi.Entities;
using Routine.APi.Services;
using Routine.Models;

namespace Routine.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId}/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public EmployeesController(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesForCompany(Guid companyId ,[FromQuery(Name = "Gender")] string genderDisplay,string q)
        {                                                                                                //進行過濾                    //進行查詢              
            if(! await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            var empolyees = await _companyRepository.GetEmployeesAsync(companyId,genderDisplay,q);
            var empolyeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(empolyees);
            return Ok(empolyeeDtos);
        }

        [HttpGet("{employeeId}",Name = nameof(GetEmployeeForCompany))]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            var empolyee = await _companyRepository.GetEmployeeAsync(companyId, employeeId);
            if(empolyee == null)
            {
                return NotFound();
            }
            var empolyeeDto = _mapper.Map<EmployeeDto>(empolyee);
            return Ok(empolyeeDto);
        }
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmpolyeeForCompany(Guid companyId,EmployeeAddDto employee)
        {
            if(!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            var entity = _mapper.Map<Employee>(employee);
            _companyRepository.AddEmployee(companyId, entity);
            await _companyRepository.SaveAsync();

            var returnDto = _mapper.Map<EmployeeDto>(entity);
            return CreatedAtRoute(nameof(GetEmployeeForCompany), new { companyId = returnDto.CompanyId, employeeId=returnDto.Id }, returnDto);
        }
        [HttpPut("{employeeId}")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployeeForCompany(
           Guid companyId,
           Guid employeeId,
           EmployeeUpdateDto employee)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId, employeeId);

            if (employeeEntity == null)
            {
                var employeeToAddEntity = _mapper.Map<Employee>(employee);
                employeeToAddEntity.Id = employeeId;

                _companyRepository.AddEmployee(companyId, employeeToAddEntity);
                await _companyRepository.SaveAsync();

                var returnDto = _mapper.Map<EmployeeDto>(employeeToAddEntity);
                return CreatedAtRoute(nameof(GetEmployeeForCompany), new { companyId = returnDto.CompanyId, employeeId = returnDto.Id }, returnDto);

            }

            // entity 转化为 updateDto
            // 把传进来的employee的值更新到 updateDto
            // 把updateDto映射回entity

            _mapper.Map(employee, employeeEntity);

            _companyRepository.UpdateEmployee(employeeEntity);

            await _companyRepository.SaveAsync();

            return NoContent();
        }
    }
}
