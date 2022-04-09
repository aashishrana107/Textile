using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Textile.Data;
using Textile.Models;

namespace Textile.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDBContext _db;
        public CategoryController(ApplicationDBContext Db)
        {
            _db = Db;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objList = _db.Category.ToList();
            return View(objList);
        }
        //Get for Create 
        public IActionResult Create()
        {
            return View();
        }
        //Post for Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {            
            if(ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    _db.Category.Add(category);
                    _db.SaveChanges();
                    scope.Complete();
                    scope.Dispose();
                    return RedirectToAction("Index");
                }
            }
            return View(category);                
        }
        //Get for Delete 
        public IActionResult Edit(int? Id)
        {
            if (Id==null||Id==0) 
            {
                return NotFound();
            }
            //Category category = _db.Category.Where(x => x.ID == Id).FirstOrDefault();
            var obj = _db.Category.Find(Id);
            if (obj==null)
            {
                return NotFound();
            }
            return View(obj);
        }
        //Post for Edit 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //var obj = _db.Category.Find(category.ID);
                    //obj.Name = category.Name;
                    //obj.DisplayOrder = category.DisplayOrder;
                    //_db.Category.Attach(obj);
                    //_db.SaveChanges();
                    _db.Category.Update(category);
                    _db.SaveChanges();
                    scope.Complete();
                    scope.Dispose();
                    return RedirectToAction("Index");
                }
            }
            return View(category);
        }

        //Get for Delete 
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _db.Category.Find(Id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //Post for Edit 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? Id)
        {
            
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var category = _db.Category.Find(Id);
            if (category == null)
            {
                return NotFound();
            }
            using (TransactionScope scope = new TransactionScope())
            {
                _db.Category.Remove(category);
                _db.SaveChanges();
                scope.Complete();
                scope.Dispose();
                return RedirectToAction("Index");
            }                      
        }
    }
}
