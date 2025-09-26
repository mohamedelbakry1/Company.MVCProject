using Company.MVCProject.BLL.Interfaces;
using Company.MVCProject.BLL.Repositories;
using Company.MVCProject.DAL.Models;
using Company.MVCProject.PL.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Company.MVCProject.PL.Controllers
{
    // MVC Controller
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _departmentRepository;

        // Ask CLR Create Object From DepartmentRepository
        public DepartmentController(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        [HttpGet] // GET: /Department/Index
        public IActionResult Index()
        {
            var department = _departmentRepository.GetAll();
            return View(department);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateDepartmentDto model)
        {
            if (ModelState.IsValid) // Server Side Validation
            {
                var department = new Department()
                {
                    Code = model.Code,
                    Name = model.Name,
                    CreateAt = model.CreateAt
                };
               var count = _departmentRepository.Add(department);
                if(count > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int? id, string ViewName = "Details")
        {
            if(id is null) return BadRequest("Invalid Id");
            var department = _departmentRepository.Get(id.Value);
            if(department is null) return NotFound($"Department with {id} Not Found");
            return View(ViewName,department);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            //if (id is null) return BadRequest("Invalid Id");
            //var department = _departmentRepository.Get(id.Value);
            //if (department is null) return NotFound($"Department with {id} Not Found");
            //return View(department);
            return Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute]int id,Department model)
        {
            if (ModelState.IsValid)
            {
                if (id != model.Id) return BadRequest("Error 404");
                var count = _departmentRepository.Update(model);
                if (count > 0)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            //if (id is null) return BadRequest("Invalid Id");
            //var department = _departmentRepository.Get(id.Value);
            //if (department is null) return NotFound($"Department with {id} Not Found");
            //return View(department);
            return Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromRoute] int id, Department model)
        {
            if (ModelState.IsValid)
            {
                if (id != model.Id) return BadRequest("Error 404");
                var count = _departmentRepository.Delete(model);
                if (count > 0)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
