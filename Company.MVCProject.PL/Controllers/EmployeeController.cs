using AutoMapper;
using Company.MVCProject.BLL.Interfaces;
using Company.MVCProject.DAL.Models;
using Company.MVCProject.PL.Dtos;
using Company.MVCProject.PL.Helpers.UploadFile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Company.MVCProject.PL.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //private readonly IEmployeeRepository _employeeRepository;
        //private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public EmployeeController(
            //IEmployeeRepository employeeRepository, 
            //IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            //_employeeRepository = employeeRepository;
            //_departmentRepository = departmentRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? SearchInput)
        {
            IEnumerable<Employee> employees;
            if (string.IsNullOrEmpty(SearchInput))
            {
                employees = await _unitOfWork.EmployeeRepository.GetAllAsync();
            }
            else
            {
                employees = await _unitOfWork.EmployeeRepository.GetByNameAsync(SearchInput);
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
        public async Task<IActionResult> Create()
        {
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            ViewData["Departments"] = departments;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeDto model)
        {
            if (ModelState.IsValid)
            {

                if(model.Image is not null)
                {
                   model.ImageName = DocumentSettings.UploadFile(model.Image, "images");
                }

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
                await _unitOfWork.EmployeeRepository.AddAsync(employee);
                var count = await _unitOfWork.CompleteAsync();
                if(count > 0)
                {
                    TempData["CreateMessage"] = "Employee Created Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id, string ViewName = "Details")
        {
            if(id is null) return BadRequest("Invalid Id");

            var employee = await _unitOfWork.EmployeeRepository.GetAsync(id.Value);
            if(employee is null) return NotFound($"Employee wtih {id} is not found");  

            

            return View(ViewName, employee);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            ViewData["Departments"] = departments;
            if (id is null) return BadRequest("Invalid Id");

            var employee = await _unitOfWork.EmployeeRepository.GetAsync(id.Value);
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
        public async Task<IActionResult> Edit([FromRoute] int id, CreateEmployeeDto model)
        {
            if (ModelState.IsValid)
            {
                if(model.ImageName is not null && model.Image is not null)
                {
                    DocumentSettings.DeleteFile(model.ImageName, "images");
                }
                if(model.Image is not null)
                {
                   model.ImageName = DocumentSettings.UploadFile(model.Image, "images");
                }
                var employee = _mapper.Map<Employee>(model);
                if (id != employee.Id) return BadRequest("Id is not matched");
                _unitOfWork.EmployeeRepository.Update(employee);
                var count = await _unitOfWork.CompleteAsync();
                if (count > 0)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, nameof(Delete));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, Employee model)
        {
            if (ModelState.IsValid)
            {
                
                if (id != model.Id) return BadRequest("Id is not matched");
                _unitOfWork.EmployeeRepository.Delete(model);
                var count = await _unitOfWork.CompleteAsync();
                if (count > 0)
                {
                    if (model.ImageName is not null)
                        DocumentSettings.DeleteFile(model.ImageName, "images");
                    TempData["DeleteMessage"] = "Employee Deleted Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }


    }
}
