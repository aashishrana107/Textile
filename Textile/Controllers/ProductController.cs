using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Textile.Data;
using Textile.Models;
using Textile.Models.ViewModels;

namespace Textile.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDBContext Db,IWebHostEnvironment WebHostEnvironment)
        {
            _db = Db;
            _webHostEnvironment = WebHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Product.Include(x=>x.Category).Include(x=>x.ApplicationType).ToList();
            //IEnumerable<Product> objList = _db.Product.ToList();
            //foreach (var product in objList)
            //{
            //    product.Category = _db.Category.FirstOrDefault(x => x.ID == product.CategoryId);
            //    product.ApplicationType = _db.ApplicationType.FirstOrDefault(x => x.ID == product.ApplicationTypeId);
            //}
            return View(objList);
        }
        //Get for Upsert 
        public IActionResult Upsert(int? Id)
        {
            //IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            //{
            //    Value = i.ID.ToString(),
            //    Text = i.Name
            //});
            //ViewBag.CategoryDropDown = CategoryDropDown;
            //ViewData["CategoryDropDown"] = CategoryDropDown; //Loosly type View
            ProductVM productVM = new ProductVM()
            {
                Product=new Product(),
                CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Value = i.ID.ToString(),
                    Text = i.Name
                }),
                ApplicationTypeSelectList = _db.ApplicationType.Select(i => new SelectListItem
                {
                    Value = i.ID.ToString(),
                    Text = i.Name
                })

            };
            if (Id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Product.Find(Id);
                if(productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
        }
        //Post for Upsert 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {            
            if(ModelState.IsValid)
            {
                
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (productVM.Product.Id==0)
                {
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload,fileName+extension),FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;
                    _db.Product.Add(productVM.Product);
                    _db.SaveChanges();
                } 
                else
                {
                    var objFormDb = _db.Product.AsNoTracking().FirstOrDefault(x => x.Id == productVM.Product.Id);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        var oldFile = Path.Combine(upload, objFormDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFormDb.Image;
                    }
                    _db.Product.Update(productVM.Product);
                    _db.SaveChanges();
                }
                return RedirectToAction("Index");                
            }
            productVM.CategorySelectList = _db.Category.Select(i => new SelectListItem
            {
                Value = i.ID.ToString(),
                Text = i.Name
            });
            productVM.ApplicationTypeSelectList = _db.ApplicationType.Select(i => new SelectListItem
            {
                Value = i.ID.ToString(),
                Text = i.Name
            });
            return View(productVM);                
        }
        
        //Get for Delete 
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            //Product product = _db.Product.Find(Id);
            //product.Category= _db.Category.Find(product.CategoryId);
            Product product = _db.Product.Include(x=>x.Category).Include(x => x.ApplicationType).FirstOrDefault(x=>x.Id==Id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //Post for Edit 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? Id)
        {
            
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var product = _db.Product.Find(Id);
            if (product == null)
            {
                return NotFound();
            }
            using (TransactionScope scope = new TransactionScope())
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                string upload = webRootPath + WC.ImagePath;
                var oldFile = Path.Combine(upload, product.Image);
                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }
                _db.Product.Remove(product);
                _db.SaveChanges();
                scope.Complete();
                scope.Dispose();
                return RedirectToAction("Index");
            }                      
        }
    }
}
