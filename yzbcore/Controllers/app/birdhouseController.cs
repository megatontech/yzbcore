using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using yzbcore.Bussiness;
using yzbcore.Models;
using yzbcore.Socket;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace yzbcore.Controllers.app
{
    //[Route("app/birdhouse")]
    [ApiController]
    public class birdhouseController : Controller
    {
        #region Public Methods
        public IBirdhouseRepository _birdhouse;
        public IEquipmentRepository _equipment;
        public IEquipment_status_logsRepository _logs;
        
        private ICache _cache;
        public birdhouseController(IBirdhouseRepository birdhouse, IEquipmentRepository equipment, ICache cache, IEquipment_status_logsRepository logs)
        {
            _birdhouse = birdhouse;
            _equipment = equipment;
            _cache = cache;
            _logs = logs;
        }

        [HttpGet]
        [Route("app/birdhouse/dataCurve")]
        public JsonResult dataCurve()
        {
            var token = Request.Query["token"][0];
            var id = Request.Query["id"][0];  
            var cache=   CacheUntity.GetCache<UserCacheModel>(token);
            dataCurve cuve = new dataCurve();
            cuve.status = "success";
            cuve.code = 1;
            cuve.data = new dataCurveData();
               var temph = _equipment.GethumidityData(id);
               var tempt = _equipment.GettemperatureData(id);
            cuve.data.humidity = new List<float>();
            cuve.data.temperature = new List<float>();
            cuve.data.humidity = temph;
            cuve.data.temperature = tempt;
            //int count = 1;
            //float counth = 0f;
            //for (int i = 0; i < 9; i++)
            //{
            //    cuve.data.humidity.Add(0);
            //}
            //foreach (var item in temph)
            //{
            //    count++;
            //    counth += item;
            //    if (count % 3 == 0) { cuve.data.humidity.Add(counth/3); }
            //}
            //int counttt = 1;
            //float counttemp = 0f;
            //foreach (var item in tempt)
            //{
            //    counttt++;
            //    counttemp += item;
            //    if (counttt % 3 == 0) { cuve.data.temperature.Add(counttemp / 3); }
            //}
            return Json(cuve); 
        }
         
        [HttpPost]
        [Route("app/birdhouse/deleteBir")]
        public JsonResult deleteBir()
        {
            var token = Request.Query["token"][0];
            var id = Request.Query["id"][0];
            //var token = value.token ;
            //var id = value.id;
            var cache = CacheUntity.GetCache<UserCacheModel>(token);
            _birdhouse.Delete(id.ToInt());
            var equip = _equipment.GetBybirdhouse_id(id.ToInt());
            equip.member_id = null;
            equip.birdhouse_id = null;
            equip.bind_status = "0";
            _equipment.Update(equip);
            var model = _cache.FillCacheWithToken(token, cache.member);
            CacheUntity.SetCache(token, model.Result);
            return Json(new { code = 1, status = "success", msg = "添加成功" });
        }

        [HttpPost]
        [Route("app/birdhouse/editBir")]
        public JsonResult editBir([FromBody] editBirModel value)
        {
            var token = value.token;
            var cache = CacheUntity.GetCache<UserCacheModel>(token);
            //var birdModelStr = value.model;
            //var birdModel = value.model;
            //JsonConvert.DeserializeObject<Birdhouse>(birdModelStr);
            Birdhouse birdhouse = _birdhouse.GetByID(value.id);
            birdhouse.update_time = Util.ToUnixStamp(DateTime.Now).ToString();
            //{"token":"08ea69102b204b09bd66cdbcf661dfef","min_temperature":"12","max_temperature":"23","min_humidity":"11"
            //,"max_humidity":"22","warning_ammonia_concentration":0,"warning_negative_pressure":0,"id":238}
            birdhouse.min_humidity = value.min_humidity.ToString("F1");
            birdhouse.min_temperature = value.min_temperature.ToString("F1");
            birdhouse.max_humidity = value.max_humidity.ToString("F1");
            birdhouse.max_temperature = value.max_temperature.ToString("F1");
            birdhouse.warning_ammonia_concentration = value.warning_ammonia_concentration.ToString();
            birdhouse.warning_negative_pressure = value.warning_negative_pressure.ToString();
            _birdhouse.Update(birdhouse);
            var model1 = _cache.FillCacheWithToken(token, cache.member);
            CacheUntity.SetCache(token, model1.Result);
            var model = new CacheModel();
            model.serial = birdhouse.equipment_id;
            var equip = _equipment.GetCacheModelByserial(model.serial);
            model.bind_status = equip.bind_status;
            model.birdhouse_name = equip.birdhouse_name;
            model.phone = 5;
            model.sms = 5;
            model.smsEnableFlag = true;
            model.call_num = 0;
            model.max_humidity = equip.max_humidity;
            model.min_humidity = equip.min_humidity;
            model.max_temperature = equip.max_temperature;
            model.min_temperature = equip.min_temperature;
            model.warning_ammonia_concentration = equip.warning_ammonia_concentration;
            model.warning_negative_pressure = equip.warning_negative_pressure;
            model.name = equip.name;
            model.status = equip.status;
            model.username = equip.username;
            model.warning_mobile = equip.warning_mobile;
            model.currentTemp = "0";
            model.currentHumidity = "0";
            model.currentPower = "0";
            model.setDate = DateTime.Now.Date;
            CacheUntity.SetCache(birdhouse.equipment_id, model);
            return Json(new { code = 1, status = "success", msg = "添加成功" });
        }

        [HttpGet]
        [Route("app/birdhouse/getBirdhouseParams")]
        public JsonResult getBirdhouseParams()
        {
            var token = Request.Query["token"][0];
            var seial = Request.Query["serial"][0];
            var equip = _equipment.GetByserial(seial);
            Equipment_status_logs data = new Equipment_status_logs();
            data.humidity = "0";
            data.temperature = "0";
            data.power_status = 0;
            var data1 = _logs.GetByserial(seial);
            if (data1 != null) { data = data1; }
            return Json(new { code = 1, status = "success", data = new List<string> { seial, data.temperature, data.humidity, data.power_status.ToString()}, msg = "添加成功" });
        }

        // GET: api/<birdhouseController>
        [HttpGet]
        [Route("app/birdhouse/index")]
        public JsonResult index()
        {
            var token = Request.Query["token"][0];
            if (string.IsNullOrEmpty(token)) { return Json(new notoken { }); }
            var id = "";
            if (Request.Query["id"].Count > 0) { id = Request.Query["id"][0]; }
            var cache = CacheUntity.GetCache<UserCacheModel>(token);

            if (string.IsNullOrEmpty(id))
            {
                birdhouseindex index = new birdhouseindex();
                index.code = "1";
                index.data = new birdhouseData();
                //if (cache.birdhouses == null|| cache.birdhouses.Count==0)
                {
                    var list = _birdhouse.GetAll().Where(x => x.member_id == cache.member.id.ToString()).ToList();
                    cache.birdhouses = list;
                }
                index.data.total = cache.birdhouses.Count;
                index.data.current_page = 1;
                index.data.last_page = 1;
                index.data.per_page = 15;
                index.data.data = new List<birdhouseDataItem>();
                foreach (var item in cache.birdhouses)
                {
                    index.data.data.Add(new birdhouseDataItem
                    {
                        id = item.id.ToInt(),
                        create_time = item.create_time,
                        equipment_id = item.equipment_id,
                        max_humidity = item.max_humidity,
                        max_temperature = item.max_temperature,
                        member_id = item.member_id.ToInt(),
                        min_humidity = item.min_humidity,
                        min_temperature = item.min_temperature,
                        name = item.name,
                        type = item.type.ToInt(),
                        warning_ammonia_concentration = item.warning_ammonia_concentration,
                        warning_negative_pressure = item.warning_negative_pressure,

                    });
                }
                index.status = "success";
                return Json(index);
            }
            else 
            {

                birdhouseindexWithid index = new birdhouseindexWithid();
                index.status = "success";
                index.code = "1";
                    var item = _birdhouse.GetAll().Where(x =>x.id==id&& x.member_id == cache.member.id.ToString()).FirstOrDefault();
                    index.data = new birdhouseDataItem
                    {
                        id = item.id.ToInt(),
                        create_time = item.create_time,
                        equipment_id = item.equipment_id,
                        max_humidity = item.max_humidity,
                        max_temperature = item.max_temperature,
                        member_id = item.member_id.ToInt(),
                        min_humidity = item.min_humidity,
                        min_temperature = item.min_temperature,
                        name = item.name,
                        type = item.type.ToInt(),
                        warning_ammonia_concentration = item.warning_ammonia_concentration,
                        warning_negative_pressure = item.warning_negative_pressure,

                    };
                return Json(index);
            }
        }

        // POST api/<birdhouseController>
        [HttpPost]
        [Route("app/birdhouse/index")]
        public JsonResult index([FromBody] Addbirdhouse value)
        {
            var token = value.token;
            var cache = CacheUntity.GetCache<UserCacheModel>(token);
            var temp = _equipment.GetByserial(value.equipment_id);
            if (temp == null)
            {
                return Json(new { code = 0, status = "fail", msg = "非法设备" });
            }
            else if(temp.bind_status=="1")
            {
                return Json(new { code = 0, status = "fail", msg = "已绑定设备" });
            }
            var equip = _equipment.GetByserial(value.equipment_id);
            Birdhouse BIRD = new Birdhouse();
            BIRD.name = value.name;
            BIRD.member_id = cache.member.id;
            BIRD.equipment_id = value.equipment_id;
            BIRD.max_humidity = "100";
            BIRD.max_temperature = "70";
            BIRD.min_humidity = "30";
            BIRD.min_temperature = "10";
            BIRD.type = value.type;
            BIRD.warning_ammonia_concentration = "0";
            BIRD.warning_negative_pressure = "0";
            BIRD.create_time = Util.ToUnixStamp(DateTime.Now).ToString();
            BIRD.update_time = Util.ToUnixStamp(DateTime.Now).ToString();
            _birdhouse.Add(BIRD);
            equip.member_id = cache.member.id;
            //equip.name = value.name;
            //equip.serial = value.equipment_id;
            equip.status = "1";
            equip.bind_status = "1";
            equip.birdhouse_id = BIRD.id;
            equip.update_time = Util.ToUnixStamp(DateTime.Now).ToString();
            _equipment.Update(equip);
            var model = _cache.FillCacheWithToken(token, cache.member);
            CacheUntity.SetCache(token, model.Result);
            return Json(new { code = 1, status = "success", msg = "添加成功" });
        }

        #endregion Public Methods
    }
    public class editBirModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int min_temperature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int max_temperature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int min_humidity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int max_humidity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int warning_ammonia_concentration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int warning_negative_pressure { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
    }
            public class editBir
    {
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public editBirModel model { get; set; }
    }
    public class deleteBir
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
    public class dataCurveData
    {
        /// <summary>
        /// 
        /// </summary>
        public List<float> temperature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<float> humidity { get; set; }
    }

    public class dataCurve
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public dataCurveData data { get; set; }
    }

    public class birdhouseDataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int member_id { get; set; }
        /// <summary>
        /// 二
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string min_temperature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string max_temperature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string min_humidity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string max_humidity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warning_ammonia_concentration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string warning_negative_pressure { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string create_time { get; set; }
    }

    public class birdhouseData
    {
        /// <summary>
        /// 
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int per_page { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int current_page { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int last_page { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<birdhouseDataItem> data { get; set; }
    }

    public class birdhouseindex
    {
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public birdhouseData data { get; set; }
    }
    public class birdhouseindexWithid
    {
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public birdhouseDataItem data { get; set; }
    }
    public class Addbirdhouse
    {
        /// <summary>
        /// {"name":"1","type":1,"equipment_id":"WZ10009","token":"c6357082fa114c8d8250d1a1cc25c344"}
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
    }

}