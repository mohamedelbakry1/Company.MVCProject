using AutoMapper;
using Company.MVCProject.BLL.Interfaces;
using Company.MVCProject.BLL.Repositories;
using Company.MVCProject.DAL.Models;
using Company.MVCProject.PL.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Company.MVCProject.PL.Controllers
{
    // MVC Controller
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        // Ask CLR Create Object From DepartmentRepository
        public DepartmentController(
            //IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            //_departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        [HttpGet] // GET: /Department/Index
        public async Task<IActionResult> Index()
        {
            var department = await _unitOfWork.DepartmentRepository.GetAllAsync();
            return View(department);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartmentDto model)
        {
            if (ModelState.IsValid) // Server Side Validation
            {
                //var department = new Department()
                //{
                //    Code = model.Code,
                //    Name = model.Name,
                //    CreateAt = model.CreateAt
                //};
                var department = _mapper.Map<Department>(model);
                await _unitOfWork.DepartmentRepository.AddAsync(department);
                var count = await _unitOfWork.CompleteAsync();
                if(count > 0)
                {
                    TempData["CreateMessage"] = "Department Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id, string ViewName = "Details")
        {
            if(id is null) return BadRequest("Invalid Id");
            var department = await _unitOfWork.DepartmentRepository.GetAsync(id.Value);
            if(department is null) return NotFound($"Department with {id} Not Found");
            return View(ViewName,department);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest("Invalid Id");
            var department = await _unitOfWork.DepartmentRepository.GetAsync(id.Value);
            if (department is null) return NotFound($"Department with {id} Not Found");
            var departmentDto = _mapper.Map<CreateDepartmentDto>(department);
            //var departmentDto = new CreateDepartmentDto()
            //{
            //    Code = department.Code,
            //    Name = department.Name,
            //    CreateAt = department.CreateAt
            //};
            return View(departmentDto);
            //return Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute]int id,Department model)
        {
            if (ModelState.IsValid)
            {
                if (id != model.Id) return BadRequest("Error 404");
                _unitOfWork.DepartmentRepository.Update(model);
                var count = await _unitOfWork.CompleteAsync();
                if (count > 0)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            //if (id is null) return BadRequest("Invalid Id");
            //var department = _departmentRepository.Get(id.Value);
            //if (department is null) return NotFound($"Department with {id} Not Found");
            //return View(department);
            return await Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, Department model)
        {
            if (ModelState.IsValid)
            {
                if (id != model.Id) return BadRequest("Error 404");
                _unitOfWork.DepartmentRepository.Delete(model);
                var count = await _unitOfWork.CompleteAsync();
                if (count > 0)
                {
                    TempData["DeleteMessage"] = "Department Deleted Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }
    }
}
