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
    //[Route("app/auth")]
    [ApiController]
    public class CallCallbackController : Controller
    {
        public ISms_logsRepository _sms_Logs;
        public ICALL _call;
        public IMemberRepository _memberRepository; private ICache _cache;
        public CallCallbackController(ISms_logsRepository sms_Logs, ICALL call, IMemberRepository memberRepository, ICache cache)
        {
            _call = call;
            _sms_Logs = sms_Logs;
            _memberRepository = memberRepository;
            _cache = cache;
        }
        private string[] status = { "未知", "关机", "空号", "停机", "正在通话中", "用户拒接", "无法拨通", "暂停服务", "用户正忙" };
        [HttpPost]
        [Route("app/CallCallback/Call")]
        public JsonResult Call([FromBody] CallCallback data)
        {
            //$tel = $data['cdr'][0]['tel'];
            //$desc = $data['cdr'][0]['stateDesc'];

            //if (in_array($desc, $this->status))
            //{
            //$info = (new \app\common\model\Member())->where('warning_mobile', 'like', "%{$tel}%")->find();

            //    Log::write($tel. ' ---- '. $desc);
            //$num = count($info['warning_mobile']);
            //    if ($num > 1 && $tel != $info['warning_mobile'][$num - 1]) {
            //    $content = json_decode(Cache::get($tel. '_warning'), true);
            //    $key = array_search($tel, $info['warning_mobile']);
            //    $sendCall = new SendCall();
            //    $status = $sendCall->templateNoticeCall($info['warning_mobile'][$key + 1], '01053182632', $content);
            //        Cache::set($info['warning_mobile'][$key + 1]. '_warning', json_encode($content));
            //        Log::write('被叫电话：'. $info['warning_mobile'][$key + 1]);
            //        Log::write('提交状态：'. $status['statusCode']. "ID：". $status['requestId']);
            //        Cache::rm($tel. '_warning');
            //    }
            //}
            //else
            //{
            //    Log::write('全部打完');
            //    Cache::rm($tel. '_warning');
            //}
            LogHelper.Error("app/CallCallback/Call"+JsonConvert.SerializeObject(data));
           var mobile = data.cdr[0].tel;
            var desc = data.cdr[0].stateDesc;
            if (status.Contains(desc)) 
            {
                LogHelper.Error("app/CallCallback/Call desc" + desc);
                var cacheModel = CacheUntity.GetCache<CacheWarningModel>(mobile);
                LogHelper.Error("app/CallCallback/Call cacheModel"  +JsonConvert.SerializeObject(cacheModel));
                if (cacheModel.uncalledmobile != null && cacheModel.uncalledmobile.Count > 0)
                {
                    var newmobile = cacheModel.uncalledmobile.First();
                    var callstatus = _call.templateNoticeCall(newmobile, "01053924437", cacheModel.birdhouse, cacheModel.content);
                    LogHelper.Error("时间: " + DateTime.Now.ToString() + "补发电话通知  " + callstatus + "被叫号码" + newmobile);
                    CacheWarningModel warncache = new CacheWarningModel();
                    warncache.mobile = newmobile;
                    warncache.mobilelist = cacheModel.mobilelist;
                    warncache.uncalledmobile = cacheModel.uncalledmobile;
                    warncache.uncalledmobile.Remove(newmobile);
                    warncache.calledmobile = cacheModel.calledmobile;
                    warncache.calledmobile.Add(newmobile);
                    warncache.birdhouse = cacheModel.birdhouse;
                    warncache.content = cacheModel.content;
                    CacheUntity.SetCache(newmobile, warncache);
                    CacheUntity.RemoveCache(mobile);
                }
                else { LogHelper.Error("全部打完"); }
            }
            else { LogHelper.Error("CallCallback 时间: " + DateTime.Now.ToString() + "电话通知状态" + desc + "requestId：" + "被叫号码" + mobile); }
           
            return Json("ok");
        }

    }
    public class CdrItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string requestid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string stime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string etime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string duration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string oriamount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string stateDesc { get; set; }
    }

    public class CallCallback
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CdrItem> cdr { get; set; }
    }
}
