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
    public class AdminController : Controller
    {
        private IAdminRepository _adminRepository;
        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
        // GET: AdminController
        public ActionResult Index()
        {
            //var model = _adminRepository.List(1,20);
            return View();
        }
        public JsonResult List() 
        {
            var limit = 0;
            var page = 0;
            limit = Convert.ToInt32(HttpContext.Request.Query["limit"]);
            page = Convert.ToInt32(HttpContext.Request.Query["page"]);
            AdminListViewModel model = new AdminListViewModel();
            var data = _adminRepository.List(page, limit);
            model.count = _adminRepository.GetAllCount();
            foreach (var item in data.Result)
            {
                item.create_time = Util.FromUnixStamp(int.Parse(item.create_time)).ToShortDateString();
            }
            model.data.AddRange(data.Result);
            return Json(model);
        }
        //public async Task<IActionResult> Index(int page = 1)
        //{
        //    page = Math.Max(1, page);

        //    var invoices = await _invoiceQueries.ListInvoices(page, 10);

        //    return View(invoices);
        //}

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
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

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
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

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
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
