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
    //[Route("app/member")]
    [ApiController]
    public class memberController : Controller
    {
        private IMemberRepository _adminRepository;
        private ICache _cache;
        public memberController(ICache cache,IMemberRepository adminRepository)
        {
            _cache = cache;
            _adminRepository = adminRepository;
        }
        // GET: api/<memberController>
        [HttpGet]
        [Route("app/member/getMemberInfo")]
        public JsonResult getMemberInfo(string token)
        {
            if (string.IsNullOrEmpty(token)) { return Json(new notoken { }); }
            memberModel model = new memberModel();
            int id = 0;
            //var cached = _cache.GetCacheByToken(token);
            var cache = CacheUntity.GetCache<UserCacheModel>(token);
            id = cache.member.id.ToInt();
                var user = _adminRepository.GetByID(id);
            model.data = new memberData
            {
                id = user.id.ToInt(),
                avatar = user.avatar,
                create_time = user.create_time,
                mobile = user.mobile,
                nick_name = user.nick_name,
                username = user.username
            };

            return Json(model);
        }

        [HttpGet]
        [Route("app/member/noticeNum")]
        public JsonResult noticeNum(string token)
        {
            if (string.IsNullOrEmpty(token)) { return Json(new notoken { }); }
            noticeNum model = new noticeNum();
            model.code = 1;
            model.status = "success";
            int id = 0;
            //var cached = _cache.GetCacheByToken(token);
            var cache = CacheUntity.GetCache<UserCacheModel>(token);
            id = cache.member.id.ToInt();
            var user = _adminRepository.GetByID(id);
            var smssum = 0;
            var call_numsum = 0;
            var phonesum = 0;
            foreach (var item in cache.birdhouses)
            {
                if (CacheUntity.Exists(item.equipment_id))
                {
                    var devicecache = CacheUntity.GetCache<CacheModel>(item.equipment_id);
                    smssum += devicecache.sms;
                    phonesum += devicecache.phone;
                }
                else {
                    smssum += 5;
                    phonesum += 5; 
                }
                
            }
            model.data = new noticeNumData
            {
                sms = smssum,
                call_num = call_numsum,
                phone = phonesum,
                smsEnableFlag = "enabled",
                warning_mobile =JsonConvert.DeserializeObject<List<string>>( cache.member.warning_mobile)
            };

            return Json(model);
        }

        // POST api/<memberController>
        [HttpPost]
        [Route("app/member/editNickName")]
        public JsonResult editNickName([FromBody] editNickName model)
        {
            var name = model.nick_name;
            var token = model.token;
            if (string.IsNullOrEmpty(token)) { return Json(new notoken { }); }
            var id = 0;
            var cache = CacheUntity.GetCache<UserCacheModel>(token);
            id = cache.member.id.ToInt();
            var user = _adminRepository.GetByID(id);
            user.nick_name = name;
            cache.member.nick_name = name;
            user.update_time = Util.ToUnixStamp(DateTime.Now).ToString();
            _adminRepository.Update(user);
            var model1 = _cache.FillCacheWithToken(token, cache.member);
            CacheUntity.SetCache(token, model1.Result);
            return Json(new memberedit());
        }

        // POST api/<memberController>
        [HttpPost]
        [Route("app/member/setWarningMobile")]
        public JsonResult setWarningMobile([FromBody] setWarningMobileModel model)
        {
            var warning_mobile = model.warning_mobile;
            var token = model.token;
            if (string.IsNullOrEmpty(token)) { return Json(new notoken { }); }
            var id = 0;
            var cache = CacheUntity.GetCache<UserCacheModel>(token);
            //var cached = _cache.GetCacheByToken(token);
            id = cache.member.id.ToInt();
            var user = _adminRepository.GetByID(id);
            //List<string> mobiles = JsonConvert.DeserializeObject<List<string>>(warning_mobile);
            //user.warning_mobile = JsonConvert.SerializeObject(mobiles);
            user.warning_mobile = JsonConvert.SerializeObject(warning_mobile, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore,  });
            user.update_time = Util.ToUnixStamp(DateTime.Now).ToString();
            if (string.IsNullOrEmpty(user.nick_name)) { user.nick_name = user.mobile; }
            if (string.IsNullOrEmpty(user.avatar)) { user.avatar = ""; }
            _adminRepository.Update(user);
            cache.member.warning_mobile = user.warning_mobile;
            var model1 = _cache.FillCacheWithToken(token, cache.member);
            CacheUntity.SetCache(token, model1.Result);
            return Json(new setWarningMobile());
        }


    }
    //如果好用，请收藏地址，帮忙分享。
    public class noticeNumData
    {
        /// <summary>
        /// 
        /// </summary>
        public int sms { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int call_num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int phone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> warning_mobile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string smsEnableFlag { get; set; }
    }

    public class noticeNum
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
        public noticeNumData data { get; set; }
    }

    public class editNickName
    {
        /// <summary>
        /// 
        /// </summary>
        public string nick_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
    }
    public class memberData
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nick_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string create_time { get; set; }
    }

    public class memberModel
    {
        public memberModel()
        {
            this.code = 1;
            this.status = "success";
           
        }

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
        public memberData data { get; set; }
    }
    public class memberedit
    {
        public memberedit()
        {
            this.code = 1;
            this.status = "success";
            data = "修改成功";
        }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 修改成功
        /// </summary>
        public string data { get; set; }
    }
    public class setWarningMobile
    {
        public setWarningMobile()
        {
            this.code = 1;
            this.status = "success";
            data = "设置成功";
        }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 修改成功
        /// </summary>
        public string data { get; set; }
    }

    public class warning_mobile
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> warning_mobilelist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
    }
    public class notoken
    {
        public notoken()
        {
            //{"code":-1,"status":"fail","msg":"没有登录"}
            this.code = -1;
            this.status = "fail";
            this.msg = "没有登录";
        }

        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 没有登录
        /// </summary>
        public string msg { get; set; }
    }
    public class setWarningMobileModel
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> warning_mobile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
    }
}
