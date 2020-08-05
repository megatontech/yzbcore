using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yzbcore.Bussiness;
using Microsoft.Extensions.Logging;
namespace yzbcore.Socket
{
    public class CALL: ICALL
    {
        //private readonly ILogger _logger;
        //云通信平台创建的 应用 AppID
        public string appID = "44a613a1fd2943da9b3e0ef6b9bfab82";
        public string version = "201512";

        //public CALL(ILogger logger)
        //{
        //    _logger = logger;
        //}

        public async Task<bool> templateNoticeCall(string mobile, string code, string birdhouse_name, string content) 
        {
            //0、电话号码 验证码 验证
            //1、构建访问参数
            string jsonData = "{\"action\":\"templateNoticeCall\",\"isdtmfcallback\":\"1\",\"dst\":\"" + mobile + "\",\"appid\":\"" + appID + "\",\"templateId\":\"2821\",\"dstclid\":\"" + code + "\",\"datas\":[\""+ birdhouse_name + "\", \""+ content + "\"]}";
            //2、云通信平台接口请求URL isdtmfcallback
            string url = "/call/NoticeCall.wx";
            LogHelper.Error("NoticeCall" + jsonData);
            //3、发送http请求，接收返回错误消息
            var result = CommenHelper.SendRequest(url, jsonData);
            LogHelper.Error("CodeCallOut" + result);
            if (result.Contains("提交成功")) { return true; }
            else { return false; }
           
        }
    }
    public interface ICALL 
    {
        /// <summary>
        /// "01053924437", cachemodel.birdhouse_name, content
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="s3"></param>
        /// <returns></returns>
        public Task<bool> templateNoticeCall(string mobile, string code, string birdhouse_name, string content);
    }
}
