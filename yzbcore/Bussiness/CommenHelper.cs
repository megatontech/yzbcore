using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;

using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace yzbcore.Bussiness
{
    public static class CommenHelper
    {
        //云通信平台的  ACCOUNT SID
          public static string accountSID = "20d3fdaadb734ac78c5aa2f865dc9b8b";
        //云通信平台的  AUTH TOKEN
          public static string authToken = "617c1a6fef4d4242945dde8aa5059b42";
        //云通信平台REST服务器IP
          public static string restIP = "http://api.ytx.net";
        //云通信平台接口版本号
       // public  string version = "201512";
        //以上参数，仅作事例，请登录云通信平台及提供的相关开发说明文档获取各参数具体值

        #region 1.0、发送http请求访问接口+ public static string SendRequest(string url, string jsonData)
        public static string SendRequest(string url, string jsonData)
        {
            string Authorization;
            string Sign;
            CommenHelper.CreatAuthorizationAndSign( out Authorization, out Sign);
            string version = "201512";
            if (url == "/call/TeleMeeting.wx" || url == "/account/getBlance.wx" || url == "/call/MeetingQuery.wx" || url == "/traffic/Traffic.wx")
            {
                version = "201612";
            }
            string urlSign = restIP + "/" + version + "/sid/" + accountSID + url + "?Sign=" + Sign;
            return SendRequest(urlSign, jsonData, Authorization);
        }
        #endregion

        #region 1.0.1、发送http请求访问接口+ public static string SendRequest(string url, string jsonData)
        public static string SendRequest(string accountsid, string authtoken, string url, string jsonData)
        {
            string Authorization;
            string Sign;
            string version = "201512";
            if (url == "/call/TeleMeeting.wx" || url == "/account/getBlance.wx" || url == "/call/MeetingQuery.wx" || url == "/traffic/Traffic.wx")
            {
                version = "201612";
            }
            CommenHelper.CreatAuthorizationAndSign(accountsid, authtoken, out Authorization, out Sign);
            string urlSign = restIP + "/" + version + "/sid/" + accountsid + url + "?Sign=" + Sign;
            return SendRequest(urlSign, jsonData, Authorization);
        }
        #endregion

        #region 1.1、发送http请求访问接口+ public static string SendRequest(string url, string jsonData, string authorization)
        /// <summary>
        /// 发送http请求访问接口
        /// </summary>
        /// <param name="url">api地址</param>
        /// <param name="jsonData">api所需请求参数json字符串</param>
        /// <returns>消息状态码</returns>
        public static string SendRequest(string url, string jsonData, string authorization)
        {
            //、将参数转化为assic码
            byte[] postBytes = Encoding.UTF8.GetBytes(jsonData);
            HttpWebRequest caaSRequest = null;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            //初始化新的webRequst
            caaSRequest = (HttpWebRequest)WebRequest.Create(url);
            caaSRequest.Method = "POST";
            caaSRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            string AuthorizationCode = authorization;
            caaSRequest.Headers.Set("Authorization", AuthorizationCode);
            caaSRequest.ContentLength = postBytes.Length;
            using (Stream reqStream = caaSRequest.GetRequestStream())
            {
                reqStream.Write(postBytes, 0, postBytes.Length);
                reqStream.Close();
            }
            string content = "";
            Stream resStream = null;
            try
            {
                HttpWebResponse caaSResponse = (HttpWebResponse)caaSRequest.GetResponse();
                resStream = caaSResponse.GetResponseStream();
                using (StreamReader sr = new StreamReader(resStream))
                {
                    content = sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse wr = (WebResponse)ex.Response;
                resStream = wr.GetResponseStream();
                using (StreamReader sr = new StreamReader(resStream))
                {
                    content = sr.ReadToEnd();
                }
            }
            return content;
        }
        #endregion
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Md5(string s)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "").ToUpper();
            }

        }

        #region 2、生成鉴权字符串+public static void CreatAuthorizationAndSign(out string Authorization, out string Sign)
        public static void CreatAuthorizationAndSign(out string Authorization, out string Sign)
        {
            string dtF = DateTime.Now.ToString("yyyyMMddHHmmss");
            //auth:账户Id + | + 时间戳
            string authFormal = string.Format("{0}|{1}", accountSID, dtF);
            Authorization = Base64EnCode(authFormal);
            //------------------------------------
            //sig:MD5加密（主帐号Id + 主帐号授权令牌 +时间戳）
            string SIGFormal = string.Format("{0}{1}{2}", accountSID, authToken, dtF);
            Sign = Md5(SIGFormal);

        } 
        #endregion

        #region 2.0、生成鉴权字符串+public static void CreatAuthorizationAndSign(string accountsid,string authtoken,out string Authorization, out string Sign)
        public static void CreatAuthorizationAndSign(string accountsid, string authtoken, out string Authorization, out string Sign)
        {
            string dtF = DateTime.Now.ToString("yyyyMMddHHmmss");
            //auth:账户Id + | + 时间戳
            string authFormal = string.Format("{0}|{1}", accountsid, dtF);
            Authorization = Base64EnCode(authFormal);
            //------------------------------------
            //sig:MD5加密（主帐号Id + 主帐号授权令牌 +时间戳）
            string SIGFormal = string.Format("{0}{1}{2}", accountsid, authtoken, dtF);
            Sign = Md5(SIGFormal); 
            //System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(SIGFormal, "md5");
        }
        #endregion

        #region 3、回调验证证书问题+private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        /// <summary>
        /// 回调验证证书问题
        /// </summary>
        /// <param name="sender">流对象</param>
        /// <param name="certificate">证书</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="errors">SslPolicyErrors</param>
        /// <returns>bool</returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        } 
        #endregion

        #region 4、Base64加密+public static string Base64EnCode(string Message)
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string Base64EnCode(string Message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Message);
            return Convert.ToBase64String(bytes);
        } 
        #endregion

        #region 5、Base64解密+public static string Base64Decode(string Message)
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string Base64Decode(string Message)
        {
            byte[] bytes = Convert.FromBase64String(Message);
            return Encoding.UTF8.GetString(bytes);
        } 
        #endregion
    }
}
