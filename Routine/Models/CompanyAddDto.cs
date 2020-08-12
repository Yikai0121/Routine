using Routine.APi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Models
{
    public class CompanyAddDto
    {
        [Display(Name ="公司名稱")]
        [Required(ErrorMessage ="請輸入{0}")] //{0}Name
        [MaxLength(100,ErrorMessage ="{0}限制為100")]
        public string Name { get; set; }
        [Display(Name ="描述")]
        [StringLength(100,MinimumLength =10,ErrorMessage ="{0}長度限制為{2}--{1}")]//{0}為屬性名稱 其餘由左到右排序
        public string Introduction { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
