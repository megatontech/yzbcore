using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using yzbcore.Bussiness;
using yzbcore.Models;
using yzbcore.Socket;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace yzbcore.Controllers.app
{
    //[Route("app/equipment")]
    [ApiController]
    public class equipmentController : Controller
    {
        public IEquipmentRepository _equipment;
        public BirdhouseRepository _bird;
        private ICache _cache;
        public equipmentController(IEquipmentRepository equipment, ICache cache)
        {
            _equipment = equipment;
            _cache = cache;
        }

        // GET: api/<equipmentController>
        [HttpGet]
        [Route("app/equipment/equipmentList")]
        public JsonResult equipmentList()
        {
            try
            {

           
            var token = Request.Query["token"][0];
            //var cached = _cache.GetCacheByToken(token);
            var cache = CacheUntity.GetCache<UserCacheModel>(token);
            var member_id = cache.member.id.ToInt();
            var list = cache.birdhouses;
            //if (list == null) 
            {
                list = _bird.GetAll().ToList().Where(x=>x.member_id==member_id.ToString()).ToList();
            }
            return Json(new { code = 1, status = "success", msg = "ok", data = list });
            }
            catch (Exception e)
            {
                LogHelper.Error(JsonConvert.SerializeObject(e));
                return Json(new nologin());
            }
        }

        // POST api/<equipmentController>
        [HttpPost]
        [Route("app/equipment/addEqu")]
        public JsonResult addEqu([FromBody] addEqu value)
        {
            try
            {

            
            var serial = value.serial;
            var token = value.token;
            //var cached = _cache.GetCacheByToken(token);
            //var member_id = cached.Result.member.id.ToInt();
            var cache = CacheUntity.GetCache<UserCacheModel>(token);
            var equip = _equipment.GetByserial(serial);
            if (equip.bind_status == "1")
            {
                return Json(new { code = 1, status = "success", msg = "此设备已被添加" });
            }
            else
            {
                //var cached = _cache.GetCacheByToken(token);
                equip.member_id = cache.member.id;
                equip.bind_status = "1";
                equip.status = "1";
                _equipment.Update(equip);
                var model = _cache.FillCacheWithToken(token, cache.member);
                CacheUntity.SetCache(token, model.Result);
                return Json(new { code = 1, status = "success", msg = "添加成功" });
            }
            }
            catch (Exception e)
            {
                LogHelper.Error(JsonConvert.SerializeObject(e));
                return Json(new nologin());
            }
        }

        // POST api/<equipmentController>
        [HttpPost]
        [Route("app/equipment/deleteEquipment")]
        public JsonResult deleteEquipment([FromBody] deleteEquipment value)
        {
            try
            {

            
            var id = value.id;
            var token = value.token;
            var cache = CacheUntity.GetCache<UserCacheModel>(token);
            var equip= _equipment.GetByID(id);
            if (string.IsNullOrEmpty(equip.birdhouse_id))
            {
                equip.member_id = "";
                equip.bind_status = "";
                equip.birdhouse_id = "";
                _equipment.Update(equip);
                var model = _cache.FillCacheWithToken(token, cache.member);
                CacheUntity.SetCache(token, model.Result);
                return Json(new { code =1, status = "success", msg = "解绑成功" });
            }
            else 
            {
                return Json(new { code = -1, status = "fail", msg = "此设备已绑定禽舍无法解绑" });
            }
            }
            catch (Exception e)
            {
                LogHelper.Error(JsonConvert.SerializeObject(e));
                return Json(new nologin());
            }
        }

    }
    
        public class addEqu
    {
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string serial { get; set; }
    }
    public class deleteEquipment
    {
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
    }
}
