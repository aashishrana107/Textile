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
    public class ApplcationTypeController : Controller
    {
        private readonly ApplicationDBContext _db;
        public ApplcationTypeController(ApplicationDBContext Db)
        {
            _db = Db;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _db.ApplicationType.ToList();
            return View(objList);
        }
        //Get for Create 
        public IActionResult Create()
        {
            return View();
        }
        //Get for Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType applicationType)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    _db.ApplicationType.Add(applicationType);
                    _db.SaveChanges();
                    scope.Complete();
                    scope.Dispose();
                    return RedirectToAction("Index");
                }
            }
            return View(applicationType);
        }
        //Get for Edit 
        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _db.ApplicationType.Find(Id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        //Post for Edit 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType applicationType)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    _db.ApplicationType.Update(applicationType);
                    _db.SaveChanges();
                    scope.Complete();
                    scope.Dispose();
                    return RedirectToAction("Index");
                }
            }
            return View(applicationType);
        }

        //Get for Delete 
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _db.ApplicationType.Find(Id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //Post for Delete 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? Id)
        {

            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var applicationType = _db.ApplicationType.Find(Id);
            if (applicationType == null)
            {
                return NotFound();
            }
            using (TransactionScope scope = new TransactionScope())
            {
                _db.ApplicationType.Remove(applicationType);
                _db.SaveChanges();
                scope.Complete();
                scope.Dispose();
                return RedirectToAction("Index");
            }
        }
    }
}
