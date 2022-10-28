using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyApp.DataAccessLayer.Data;
using MyApp.DataAccessLayer.Infrastructure.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModel;
using System.Runtime.CompilerServices;

namespace Practice1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        //private ApplicationDBContext _context;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        #region APICall
        [HttpGet]
        public IActionResult AllProducts()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties:"Category");
            return Json(new { data = products });
        }
        #endregion

        public IActionResult Index()
        {
            ProductVM prodVM = new ProductVM();
            prodVM.products = _unitOfWork.Product.GetAll();

            return View();
        }
        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            ProductVM prodVM = new ProductVM()
            {
                Product = new(),
                Categories = _unitOfWork.Category.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            if (id == null || id == 0)
            {
                return View(prodVM);
            }
            prodVM.Product = _unitOfWork.Product.GetT(x => x.Id == id);
            if (prodVM.Product == null)
            {
                return NotFound();
            }
            return View(prodVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(ProductVM prodVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "ProductImages");
                    var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                    if(prodVM.Product.ImageUrl!=null)
                        {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, prodVM.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    prodVM.Product.ImageUrl = @"\ProductImages\" + fileName;
                }
                if (prodVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(prodVM.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product Created Successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(prodVM.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product Updated Successfully";
                }
                return RedirectToAction("Index");
            }
            TempData["error"] = "Some Error Occurred";
            return View(prodVM);
        }


        //[HttpGet]
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null && id == 0)
        //    {
        //        TempData["error"] = "Some Error Occurred";
        //        return NotFound();
        //    }
        //    var Product = _unitOfWork.Product.GetT(x => x.Id == id);
        //    if (Product == null)
        //    {
        //        TempData["error"] = "Some Error Occurred";
        //        return NotFound();
        //    }
        //    _unitOfWork.Product.Delete(Product);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product Deleted Successfully";
        //    return RedirectToAction("Index");
        //}
        #region DeleteAPICall
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id == null && id == 0)
            {
                return Json(new { success = false, Error = "Error In Fetching Data" });
            }
            var Product = _unitOfWork.Product.GetT(x => x.Id == id);
            if (Product == null)
            {
                return Json(new {success=false, Error="Error In Fetching Data"});
            }
            else
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, Product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                _unitOfWork.Product.Delete(Product);
                _unitOfWork.Save();
                return Json(new { success = true, Error = "Data Deleted Successfully" });
            }
        }
        #endregion
    }
}
