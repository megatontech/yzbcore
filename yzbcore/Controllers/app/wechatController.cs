using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using yzbcore.Bussiness;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace yzbcore.Controllers.app
{
    //[Route("app/wechat")]
    [ApiController]
    public class wechatController : Controller
    {
        // GET: api/<wechatController>
        [HttpGet]
        [Route("app/wechat/scan")]
        public JsonResult scan()
        {
            scanmodel s = new scanmodel();
            // { "debug":true,"beta":false,"jsApiList":["sacnQRcode"],"appId":"wx1d580eff165affc1","nonceStr":"6uIpP11x6q","timestamp":1596340111,
            //"url":"http:\/\/h5.yzb.zhongzexinxi.com\/","signature":"7ee50ff826ba05b887e7f0cf01d5afa8d87e6cdd"}
            s.debug = true;
            s.beta = false;
            s.jsApiList = new List<string>();
            s.jsApiList.Add("sacnQRcode");
            s.appId = "wx2a5052a9b7498da7";
            s.nonceStr = WeHelper.CreateNonceStr();
            s.timestamp = WeHelper.CreateTimestamp();
            //s.url = @"http:\/\/www.michat520.cn\/";
            //s.url = @"www.michat520.cn";
            s.url = @"http://www.michat520.cn/";
            s.signature = WeHelper.GetSignature(s.timestamp, s.nonceStr, s.url);
            return Json(s);
        }
    }
    public class scanmodel
    {
        /// <summary>
        /// 
        /// </summary>
        public bool debug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool beta { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> jsApiList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string appId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nonceStr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string signature { get; set; }
    }



     public static class WeHelper
    {
        //        return [
        //    'app_id' => '开发者ID(AppID)    wx1d580eff165affc1',
        //    'secret' => 'ed354ceff01115fd6d88c2960d775c75',
        //    'token'  => '9NG3BemAHzLQqpXp4ptT6ojHYk7WNY4p',
        ////    'token'  => 'apptest',
        //    'aes_key' => '',
        //    'response_type' => 'array',
        //];
        public static string appId
        {
            get
            {
                //return "wx1d580eff165affc1";//微信开发者appId
                return "wx2a5052a9b7498da7";//微信开发者appId
            }
        }
        private static string secret
        {
            get
            {
                
                //return "ed354ceff01115fd6d88c2960d775c75";//微信开发者secret
                return "43890589c01248dafa3e3d66e17ea785";//微信开发者secret
            }
        }

        private static readonly string tokenUrl = "https://api.weixin.qq.com/cgi-bin/token";

        private static readonly string ticket = "https://api.weixin.qq.com/cgi-bin/ticket/getticket";

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            //if (HttpRuntime.Cache.Get("AccessToken") == null)
            {
                StringBuilder sbUrl = new StringBuilder();
                sbUrl.AppendFormat(tokenUrl);
                sbUrl.AppendFormat("?grant_type=client_credential");
                sbUrl.AppendFormat("&appid={0}", appId);
                sbUrl.AppendFormat("&secret={0}", secret);
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    string data = client.UploadString(sbUrl.ToString(), string.Empty);
                    var result = JObject.Parse(data);
                    if (result["access_token"] != null && result["access_token"].Value<string>() != string.Empty)
                    {
                        string token = result["access_token"].Value<string>();
                        //HttpRuntime.Cache.Insert("AccessToken", token, null, DateTime.Now.AddSeconds(7200), System.Web.Caching.Cache.NoSlidingExpiration);
                        return token;
                    }
                    else
                        throw new Exception(data);
                }
            }
            //else
            //{
               // return HttpRuntime.Cache.Get("AccessToken").ToString();
           // }
        }
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string GetTicket(string accessToken)
        {
            //if (HttpRuntime.Cache.Get("Ticket") == null)
            {
                StringBuilder sbUrl = new StringBuilder();
                sbUrl.AppendFormat(ticket);
                sbUrl.AppendFormat("?access_token={0}", accessToken);
                sbUrl.AppendFormat("&type=jsapi");
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    string data = client.UploadString(sbUrl.ToString(), string.Empty);
                    LogHelper.Error(data);
                    var result = JObject.Parse(data);
                    if (result["ticket"] != null && result["ticket"].Value<string>() != string.Empty)
                    {
                        string ticket = result["ticket"].Value<string>();
                        //HttpRuntime.Cache.Insert("Ticket", ticket, null, DateTime.Now.AddSeconds(7200), System.Web.Caching.Cache.NoSlidingExpiration);
                        return ticket;
                    }
                    else
                        throw new Exception(data);
                }
            }
            //else
            //{
            //    return HttpRuntime.Cache.Get("Ticket").ToString();
            //}
        }
        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="noncestr"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetSignature(string timestamp, string noncestr, string url)
        {
            string string1 = "jsapi_ticket=" + GetTicket(GetAccessToken()) + "&noncestr=" + noncestr + "&timestamp=" + timestamp + "&url=" + url;
            //使用sha1加密这个字符串
            return SHA1(string1);
        }
        #region 工具类
        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <returns></returns>
        public static string CreateNonceStr()
        {
            int length = 16;
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string str = "";
            var rad = new Random();
            for (int i = 0; i<length; i++)
            {
                str += chars.Substring(rad.Next(0, chars.Length - 1), 1);
            }
            return str;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SHA1(string str)
        {
            str = SHA1Encrypt(str).ToString();
            return str.ToLower();
        }
        #region 用SHA1加密字符串

        /// <summary>
        /// 用SHA1加密字符串
        /// </summary>
        /// <param name="source">要扩展的对象</param>
        /// <param name="isReplace">是否替换掉加密后的字符串中的"-"字符</param>
        /// <param name="isToLower">是否把加密后的字符串转小写</param>
        /// <returns></returns>
        public static string SHA1Encrypt(this string source, bool isReplace = true, bool isToLower = false)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            //SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(source));
            string shaStr = BitConverter.ToString(hash);
            if (isReplace)
            {
                shaStr = shaStr.Replace("-", "");
            }
            if (isToLower)
            {
                shaStr = shaStr.ToLower();
            }
            return shaStr;
        }

        #endregion
        /// <summary>
        /// 生成时间戳
        /// </summary>
        /// <returns></returns>
        public static string CreateTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timestamp = Convert.ToInt64(ts.TotalSeconds).ToString();
            return timestamp;
        } 
        #endregion
    }


}
