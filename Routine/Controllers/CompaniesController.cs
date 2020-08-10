using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Routine.APi.Entities;
using Routine.APi.Services;
using Routine.DtoParameters;
using Routine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 * HTTP请求：
 * GET      - 查询
 * POST     - 创建/添加
 * PATCH    - 局部修改/更新
 * PUT      - 如果存在就替换，不存在则创建
 * DELETE   - 移除/删除
 */

/*
 * 返回状态码：
 * 
 * 2xx 成功
 * 200 - OK 請求成功
 * 201 - Created 請求成功建了資源
 * 204 - No Content 請求成功，但是不應該返回任何東西，例如刪除操作
 * 
 * 4xx 客户端錯误
 * 400 - Bad Request API發送到伺服器的請求是有錯误的
 * 401 - Unauthorized 没有提供授權信息或者提供的授權信息不正确
 * 403 - Forbidden 身份認證已經成功，但是已認證的用戶卻無法訪問请求的资源
 * 404 - Not Found 請求的資源不存在
 * 405 - Method Not Allowed 常是發送請求到資源的时候，使用了不被支持的HTTP方法
 * 406 - Not Acceptable API請求的表述格式不被Web API所支持，并且API不
 *       會提供默認的表述格式
 * 409 - Conflict 請求伺服器當前狀態衝突（通常指更新資源發生的衝突）
 * 415 - Unsupported Media Type 與406正好相反，有一些请求必須帶著数据发往伺服器，
 *       這些數據都属于特定的媒體格式，如果API不支持該媒體类型格式，415就會被返回
 * 422 - Unprocessable Entity 它是HTTP拓展協議的一部分，它說明伺服器已经懂得了
 *       Content Type，實體的語法也没有问题，但是伺服器無法處理這個實體数据 
 * 
 * 5xx 伺服器錯误
 * 500 - Internal Server Error 伺服器出現錯誤
 */

namespace Routine.APi.Controllers
{
    /*[ApiController]属性並不是强制要求的，但是它會使發開體驗更好
     * 它会启用以下行为：
     * 1.要求使用属性路由（Attribute Routing）
     * 2.自動HTTP 400響應
     * 3.推断参数的绑定源
     * 4.Multipart/form-data请求推断
     * 5.錯誤狀態代碼的問題詳細信息
     */
    [ApiController]
    [Route("api/companies")] //可用 [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompaniesController(ICompanyRepository companyRepository, IMapper mapper)
        {
            this._companyRepository = companyRepository ??
                                        throw new ArgumentNullException(nameof(companyRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [HttpHead]
        //public async Task<IActionResult> GetCompanies() 與下方相同但下方更明確
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies([FromQuery] CompanyDtoParameters parameters)
        {
            
            var companies = await _companyRepository.GetCompaniesAsync(parameters);
            //                       目標:IEnumerable<CompanyDto>來源companies
            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companyDtos);  //OK() 返回200
        }

        [HttpGet("{companyId}",Name = nameof(GetCompany))]  //可用 [Route("{companyId}")]
        public async Task<ActionResult<CompanyDto>> GetCompany(Guid companyId)
        {
            //較不適合的方法：
            //var exist = await _companyRepository.CompanyExistsAsync(companyId);
            //if (!exist)
            //{
            //    return NotFound(); //返回404
            //}
            //var company = await _companyRepository.GetCompanyAsync(companyId);
            //return Ok(company);
            //

            //略有改善的方法：
            var company = await _companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return NotFound();  //返回404
            }
            //      目標:<CompanyDto>來源company
            var companyDtos = _mapper.Map<CompanyDto>(company);
            return Ok(companyDtos);
        }
        [HttpPost]
        //CompanyAddDto加入public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        //同時創建empolyee子類資源
            //         {
            //     "name":"Asus",
            //     "introduction":"A Good Company",
            //     "employees":[
            //         {
            //             "employeeNo":"F001",
            //            "firstName":"White",
            //            "lastName":"Face",
            //            "gender":0,
            //            "dataofBirth":"1994-8-3"
            //         },
            //          {
            //             "employeeNo":"F011",
            //            "firstName":"Black",
            //            "lastName":"Face",
            //            "gender":1,
            //            "dataofBirth":"1994-12-3"
            //         }
            //     ]
    
            //}
        public async Task<ActionResult<CompanyDto>> CreatCompany([FromBody] CompanyAddDto company)
        {
            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();

            var returnDto = _mapper.Map<CompanyDto>(entity);
            return CreatedAtRoute(nameof(GetCompany), new { companyId = returnDto.Id }, returnDto);
        }
        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {

        }
    }
}
