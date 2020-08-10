using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Routine.APi.Entities;
using Routine.APi.Services;
using Routine.Helpers;
using Routine.Models;

namespace Routine.Controllers
{
    [ApiController]
    [Route("api/companycollents")]
    public class CompanyCollectionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;
        public CompanyCollectionsController(IMapper mapper,ICompanyRepository companyRepository)
        {
            this._companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(_companyRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        }
        //uri後面的參數表示為/ex1,ex2,ex3,ex4
        [HttpGet("({ids})",Name = nameof(GetCompanyCollection))]
        public async Task<IActionResult> GetCompanyCollection([FromRoute][ModelBinder(BinderType =typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }
            var entities = await _companyRepository.GetCompaniesAsync(ids);
            
            if (ids.Count() != entities.Count())
            {
                return NotFound();
            }
            var dtosreturn = _mapper.Map<IEnumerable<CompanyDto>>(entities);
            return Ok(dtosreturn);
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> CreateCompanyCollention(IEnumerable<CompanyAddDto> companyCollention)
        {
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollention);
            foreach (var company in companyEntities)
            {
                _companyRepository.AddCompany(company);
            }
            await _companyRepository.SaveAsync();

            var dtos = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

            var idsString = string.Join(",", dtos.Select(x => x.Id));

            return CreatedAtRoute(nameof(GetCompanyCollection), new { ids = idsString }, dtos);
        }
    }
}
