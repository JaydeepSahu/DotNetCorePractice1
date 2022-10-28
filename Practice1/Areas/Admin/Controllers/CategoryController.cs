using Microsoft.AspNetCore.Mvc;
using MyApp.DataAccessLayer.Data;
using MyApp.DataAccessLayer.Infrastructure.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModel;

namespace Practice1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        //private ApplicationDBContext _context;
        private IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult Index()
        {
            CategoryVM catVM = new CategoryVM();
            catVM.categories = _unitOfWork.Category.GetAll();

            return View(catVM);
        }
       
        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            CategoryVM catVM = new CategoryVM();
            if (id == null || id == 0)
            {
                return View(catVM);
            }
            catVM.Category = _unitOfWork.Category.GetT(x => x.Id == id);
            if(catVM.Category == null)
            {
                return NotFound();
            }
            return View(catVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(CategoryVM catVM)
        {
            if (ModelState.IsValid)
            {
                if(catVM.Category.Id == 0)
                {
                    _unitOfWork.Category.Add(catVM.Category);
                    _unitOfWork.Save();
                    TempData["success"] = "Category Created Successfully";
                }
                else
                {
                    _unitOfWork.Category.Update(catVM.Category);
                    _unitOfWork.Save();
                    TempData["success"] = "Category Updated Successfully";
                }
                return RedirectToAction("Index");
            }
            TempData["error"] = "Some Error Occurred";
            return View(catVM);
        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null && id == 0)
            {
                TempData["error"] = "Some Error Occurred";
                return NotFound();
            }
            var category = _unitOfWork.Category.GetT(x => x.Id == id);
            if (category == null)
            {
                TempData["error"] = "Some Error Occurred";
                return NotFound();
            }
            _unitOfWork.Category.Delete(category);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
