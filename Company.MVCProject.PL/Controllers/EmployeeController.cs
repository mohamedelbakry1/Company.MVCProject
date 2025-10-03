using AutoMapper;
using Company.MVCProject.BLL.Interfaces;
using Company.MVCProject.DAL.Models;
using Company.MVCProject.PL.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Company.MVCProject.PL.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public EmployeeController(
            IEmployeeRepository employeeRepository, 
            IDepartmentRepository departmentRepository,
            IMapper mapper
            )
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Index(string? SearchInput)
        {
            IEnumerable<Employee> employees;
            if (string.IsNullOrEmpty(SearchInput))
            {
                employees = _employeeRepository.GetAll();
            }
            else
            {
                employees = _employeeRepository.GetByName(SearchInput);
            }
            // Dictionary : 3 Property
            // 1.ViewData : Transfer Extra Information From Controller (Action) to View
            //ViewData["Message"] = "Hello from ViewData";


            // 2.ViewBag : Transfer Extra Information From Controller (Action) to View
            //ViewBag.Message = "Hello from ViewBag";
            //ViewBag.Message = new { Message = "Hello from ViewBag" };
            // 3.TempData
            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var departments = _departmentRepository.GetAll();
            ViewData["Departments"] = departments;
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateEmployeeDto model)
        {
            if (ModelState.IsValid)
            {
                // Manual Mapping
                //var employee = new Employee()
                //{
                //    Name = model.Name,
                //    Age = model.Age,
                //    Email = model.Email,
                //    Address = model.Address,
                //    Phone = model.Phone,
                //    Salary = model.Salary,
                //    IsActive = model.IsActive,
                //    IsDeleted = model.IsDeleted,
                //    HiringDate = model.HiringDate,
                //    CreateAt = model.CreateAt,
                //    DepartmentId = model.DepartmentId
                //};
                var employee = _mapper.Map<Employee>(model);
                var count = _employeeRepository.Add(employee);
                if(count > 0)
                {
                    TempData["CreateMessage"] = "Employee Created Successfully";
                    return RedirectToAction("Index");
                }
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
            var departments = _departmentRepository.GetAll();
            ViewData["Departments"] = departments;
            if (id is null) return BadRequest("Invalid Id");

            var employee = _employeeRepository.Get(id.Value);
            if (employee is null) return NotFound($"Employee wtih {id} is not found");
            //var employeeDto = new CreateEmployeeDto()
            //{
            //    Name = employee.Name,
            //    Age = employee.Age,
            //    Email = employee.Email,
            //    Address = employee.Address,
            //    Phone = employee.Phone,
            //    Salary = employee.Salary,
            //    IsActive = employee.IsActive,
            //    IsDeleted = employee.IsDeleted,
            //    HiringDate = employee.HiringDate,
            //    CreateAt = employee.CreateAt
            //};
            var employeeDto = _mapper.Map<CreateEmployeeDto>(employee);
            return View(employeeDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int id, Employee model)
        {
            if (ModelState.IsValid)
            {
                if (id != model.Id) return BadRequest("Id is not matched");
                var count = _employeeRepository.Update(model);
                if(count > 0)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
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
                {
                    TempData["DeleteMessage"] = "Employee Deleted Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(employee);
        }


    }
}
