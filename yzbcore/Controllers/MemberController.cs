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
    public class MemberController : Controller
    {
        private IMemberRepository _adminRepository;
        public MemberController(IMemberRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        // GET: MemberController
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult List()
        {
            var limit = 0;
            var page = 0;
            limit = Convert.ToInt32(HttpContext.Request.Query["limit"][0]);
            page = Convert.ToInt32(HttpContext.Request.Query["page"][0]);
            MemberListViewModel model = new MemberListViewModel();
            var data = _adminRepository.List(page, limit);
            model.count = _adminRepository.GetAllCount();
            foreach (var item in data.Result)
            {
                item.create_time = Util.FromUnixStamp(int.Parse(item.create_time)).ToShortDateString();
            }
            model.data.AddRange(data.Result);
            return Json(model);
        }
        // GET: MemberController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MemberController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MemberController/Create
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

        // GET: MemberController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MemberController/Edit/5
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

        // GET: MemberController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MemberController/Delete/5
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
