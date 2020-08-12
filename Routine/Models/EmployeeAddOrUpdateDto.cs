using Routine.APi.Entities;
using Routine.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Models
{
    
    [EmployeeNoMustDifferentFromFirstNameAttribute(ErrorMessage = "工號和名字不能相同")]
    public abstract class EmployeeAddOrUpdateDto : IValidatableObject
    {

        [Display(Name = "員工工號")]
        [Required(ErrorMessage = "{0}必填寫")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0}的長度為{1}")]
        public string EmployeeNo { get; set; }
        [Display(Name = "名")]
        [Required(ErrorMessage = "{0}必填寫")]
        [MaxLength(50, ErrorMessage = "{0}長度不能超過{1}")]
        public string FirstName { get; set; }
        [Display(Name = "姓")]
        [Required(ErrorMessage = "{0}必填寫")]
        [MaxLength(50, ErrorMessage = "{0}長度不能超過{1}")]
        public string LastName { get; set; }
        [Display(Name = "性別")]
        public Gender Gender { get; set; }
        [Display(Name = "生日")]
        public DateTime DateOfBirth { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FirstName == LastName)
            {
                yield return new ValidationResult("姓和名不能相同", new[] { nameof(FirstName), nameof(LastName) });
            }
        }
    }
}
