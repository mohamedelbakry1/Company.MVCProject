using Company.MVCProject.DAL.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Company.MVCProject.PL.Dtos
{
    public class CreateEmployeeDto
    {
        [Required(ErrorMessage = "Name is required !!")]
        public string Name { get; set; }
        [Range(22,30,ErrorMessage ="Age Must be Between 22 and 60")]
        public int? Age { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage ="Email is not valid !!")]
        public string Email { get; set; }
        //[RegularExpression(@"[0-9]{1,3}-[a-zA-Z]{5,10}[a-zA-Z]{4,10}[a-zA-Z]{5,10}$",
        //    ErrorMessage = "Address must be like 123-street-city-country")]
        public string Address { get; set; }
        [Phone]
        public string Phone { get; set; }
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [DisplayName("Hiring Data")]
        public DateTime HiringDate { get; set; }
        [DisplayName("Date of Creation")]
        public DateTime CreateAt { get; set; }
        [DisplayName("Department")]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
