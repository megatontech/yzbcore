using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using yzbcore.Bussiness;
using yzbcore.Models;

namespace yzbcore.Controllers
{
    public class EquipmentController : Controller
    {

        private IEquipmentRepository _adminRepository;
        public EquipmentController(IEquipmentRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
        // GET: EquipmentController
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult List()
        {
            var limit = 0;
            var page = 0;
            limit = Convert.ToInt32(HttpContext.Request.Query["limit"]);
            page = Convert.ToInt32(HttpContext.Request.Query["page"]);
            EquipListViewModel model = new EquipListViewModel();
            var data = _adminRepository.List(page, limit);
            model.count = _adminRepository.GetAllCount();
            foreach (var item in data.Result)
            {
                item.create_time = Util.FromUnixStamp(int.Parse(item.create_time)).ToShortDateString();
            }
            model.data.AddRange(data.Result);
            return Json(model);
        }
        // GET: EquipmentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EquipmentController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EquipmentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EquipmentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EquipmentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EquipmentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EquipmentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
