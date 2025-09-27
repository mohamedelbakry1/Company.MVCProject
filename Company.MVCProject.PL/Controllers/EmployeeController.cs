using Company.MVCProject.BLL.Interfaces;
using Company.MVCProject.DAL.Models;
using Company.MVCProject.PL.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Company.MVCProject.PL.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var employees = _employeeRepository.GetAll();
            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {
           return View();
        }

        [HttpPost]
        public IActionResult Create(CreateEmployeeDto model)
        {
            if (ModelState.IsValid)
            {
                var employee = new Company.MVCProject.DAL.Models.Employee
                {
                    Name = model.Name,
                    Age = model.Age,
                    Email = model.Email,
                    Address = model.Address,
                    Phone = model.Phone,
                    Salary = model.Salary,
                    IsActive = model.IsActive,
                    IsDeleted = model.IsDeleted,
                    HiringDate = model.HiringDate,
                    CreateAt = model.CreateAt
                };
                var count = _employeeRepository.Add(employee);
                if(count > 0)
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int? id, string ViewName = "Details")
        {
            if(id is null) return BadRequest("Invalid Id");

            var employee = _employeeRepository.Get(id.Value);
            if(employee is null) return NotFound($"Employee wtih {id} is not found");  
            return View(ViewName, employee);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            return Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int id, Employee employee)
        {
            if (ModelState.IsValid)
            {
                if(id != employee.Id) return BadRequest("Id is not matched");
                var count = _employeeRepository.Update(employee);
                if(count > 0)
                    return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            return Details(id, nameof(Delete));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromRoute] int id, Employee employee)
        {
            if (ModelState.IsValid)
            {
                if (id != employee.Id) return BadRequest("Id is not matched");
                var count = _employeeRepository.Delete(employee);
                if (count > 0)
                    return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }


    }
}
