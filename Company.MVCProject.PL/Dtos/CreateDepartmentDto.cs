using System.ComponentModel.DataAnnotations;

namespace Company.MVCProject.PL.Dtos
{
    public class CreateDepartmentDto
    {
        [Required(ErrorMessage = "Code is Required")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Date of Creation is Required")]
        public DateTime CreateAt { get; set; }
    }
}
