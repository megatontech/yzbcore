using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using yzbcore.Bussiness;
using yzbcore.Socket;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace yzbcore.Controllers.app
{
    //[Route("app/auth")]
    [ApiController]
    public class authController : Controller
    {
        public ISms_logsRepository _sms_Logs;
        public IMemberRepository _memberRepository; private ICache _cache;
        public authController(ISms_logsRepository sms_Logs, IMemberRepository memberRepository, ICache cache)
        {
            _sms_Logs = sms_Logs;
            _memberRepository = memberRepository;
            _cache = cache;
        }

        // GET: api/<authController>
        [HttpGet]
        [Route("app/auth/login")]
        public JsonResult login()
        {
            var mobile = Request.Query["mobile"][0];
            var code = Request.Query["code"][0];
            if (code == _sms_Logs.GetBymobile(mobile).code&& _sms_Logs.GetBymobile(mobile).end_time.ToInt()>=Util.ToUnixStamp(DateTime.Now)) 
            {
                if (_memberRepository.GetBymobile(mobile)!= null) { }
                else {
                    _memberRepository.Add(new Models.Member
                    {
                        create_time = Util.ToUnixStamp(DateTime.Now).ToString()
                    ,
                        mobile = mobile,
                        username = mobile
                        ,
                        nick_name = mobile,
                        update_time = Util.ToUnixStamp(DateTime.Now).ToString(),
                        warning_mobile = ""
                    }) ;
                }
                
                var user = _memberRepository.GetBymobile(mobile);
                var token = Guid.NewGuid().ToString().Replace("-","");
                var model = _cache.FillCacheWithToken(token,user);
                //string res = CacheUntity.GetCache<string>("test");

                CacheUntity.Init(new RedisCacheHelper());
                CacheUntity.SetCache(token, model.Result);
                //_cache.InsertCache(model.Result);
                login log = new login();
                log.code = "1";
                log.status = "success";
                log.data = new loginData { code = code, create_time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), id = user.id.ToInt(), mobile = mobile, update_time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), end_time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), token = token };
                return Json(log);
            }
            else {
                loginerr logerr = new loginerr();
                logerr.code = "-1";
                logerr.status = "fail";
                logerr.msg = "验证码错误";
                return Json(logerr);
            }
            
        }

    }
    //如果好用，请收藏地址，帮忙分享。
    public class loginerr
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
        /// 验证码错误
        /// </summary>
        public string msg { get; set; }
    }
    //如果好用，请收藏地址，帮忙分享。
    public class loginData
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string end_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string create_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string update_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
    }

    public class login
    {
        public login()
        {
            this.code = "1";
            this.status = "success";
        }

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
        public loginData data { get; set; }
    }

}
