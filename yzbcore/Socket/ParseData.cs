using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yzbcore.Bussiness;
using yzbcore.Models;

namespace yzbcore.Socket
{
    public interface IParseData
    {
        #region Public Methods

        public Task ProcessDATA(string data);

        #endregion Public Methods
    }

    public class ParseData : IParseData
    {
        #region Public Fields

        //public ICache _cache;
        public IEquipment_status_logsRepository _logsRepo;
        public IWSSendData _sendData;
        public ISMS _sms;
        public ICALL _call;
        public IWECHAT _wechat;
        public IEquipmentRepository _equipment;
        public IBirdhouseRepository _birdhouse;
        #endregion Public Fields



        #region Public Constructors

        public ParseData(IWSSendData sendData, //ICache cache,
            IEquipment_status_logsRepository logsRepo, IBirdhouseRepository birdhouse, IEquipmentRepository equipment, ISMS sms, ICALL call, IWECHAT wechat)
        {
            _sendData = sendData;
            //_cache = cache;
            _equipment = equipment;
            _logsRepo = logsRepo; 
            _sms = sms;
            _call = call;
            _wechat = wechat;
            _birdhouse = birdhouse;
        }

     

        #endregion Public Constructors



        #region Public Methods

        public async Task ProcessDATA(string data)
        {
            if (!string.IsNullOrEmpty(data)&& data.Contains(","))
            {
                var arr = data.Split(',');
                if (data.EndsWith("\r\n"))
                {
                    var newSTR = arr[3].Replace("\r\n", "");
                    arr[3] = newSTR;
                }
                _sendData.SendToUid(arr[0], JsonConvert.SerializeObject(arr));
                LogHelper.Error("发送数据到ws" + arr[0] + "内容" + JsonConvert.SerializeObject(arr));
                var cache = CacheUntity.GetCache<CacheModel>(arr[0]);
                if (cache == null|| cache.setDate<DateTime.Now.Date) 
                {
                    if (cache == null) { LogHelper.Error("cache null "); }
                    else { LogHelper.Error("cache  fail" + JsonConvert.SerializeObject(cache)); }
                   
                    var model = new CacheModel();
                    model.serial = arr[0];
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
                    model.currentTemp = arr[1];
                    model.currentHumidity= arr[2];
                    model.currentPower = arr[3];
                    model.setDate = DateTime.Now.Date;
                    CacheUntity.SetCache(arr[0], model);
                    cache = model;
                }
                //找到对应设备
                if (!string.IsNullOrEmpty(cache.warning_mobile) && cache.serial == arr[0])
                {
                    var cachemodel = cache;
                    var equipmentStatus = 0;
                    if (!string.IsNullOrEmpty(cachemodel.min_temperature) && !string.IsNullOrEmpty(cachemodel.warning_mobile))
                    {
                        if (arr[1].ToFloat() < cachemodel.min_temperature.ToFloat() || arr[1].ToFloat() > cachemodel.max_temperature.ToFloat())
                        {
                            equipmentStatus += 10;
                            LogHelper.Error("temperature "+ arr[1] + "min_temperature"+ cachemodel.min_temperature + "max_temperature"+ cachemodel.max_temperature);
                        }
                    }

                    if (!string.IsNullOrEmpty(cachemodel.min_humidity) && !string.IsNullOrEmpty(cachemodel.max_humidity))
                    {
                        if (arr[2].ToFloat() < cachemodel.min_humidity.ToFloat() || arr[2].ToFloat() > cachemodel.max_humidity.ToFloat())
                        {
                            equipmentStatus += 20;
                            LogHelper.Error("temperature " + arr[2] + "min_humidity" + cachemodel.min_humidity + "max_humidity" + cachemodel.max_humidity);
                        }
                    }

                    ////如果之前温室度出现过异常，只有温度和湿度全部改为正常后，才将可发送短息标志位 重置为 可启用
                    if (equipmentStatus == 0)
                    {
                    //如果温湿度正常，则将启用短信发送  重新置为 可用
                        cachemodel.smsEnableFlag = true;
                        //_cache.UpdateCache(cachemodel);
                        CacheUntity.SetCache(arr[0], cachemodel);
                    }

                    //// 判断设备是否断电
                    if (arr[3].ToInt() == 1) {
                        equipmentStatus += 40;
                    }

                    //获取第一个报警电话
                    var mobile = JsonConvert.DeserializeObject<List<string>>(cachemodel.warning_mobile).First();
                    var mobileList = JsonConvert.DeserializeObject<List<string>>(cachemodel.warning_mobile);
                    //    //进行报警
                    switch (equipmentStatus)
                    {
                        case 10:
                            //判断短信剩余数量，以及短信发送标识，如果为enabled则发送
                            if (cachemodel.sms > 0 && cachemodel.smsEnableFlag == true) {
                                //发送短信
                                var status = _sms.sendSms(mobile, 2, cachemodel.birdhouse_name, "温度");
                                if (status.Result) {
                                    cachemodel.sms -= 1;
                                    //异常则将短信启用发送标志位置为 不可用，也就是不再发送短信，除非重新接受到设备发来的数据判断为正常则
                                    //重新将短信启用标志位 置为  可用
                                    cachemodel.smsEnableFlag = false;
                                    //_cache.UpdateCache(cachemodel);
                                    CacheUntity.SetCache(arr[0], cachemodel);
                                    LogHelper.Error("温度-短信：数据" +data+"设置"+ JsonConvert.SerializeObject(cachemodel));
                                }
                            }
                            
                            break;
                        case 20:
                            if (cachemodel.sms > 0 && cachemodel.smsEnableFlag == true) {
                                var status = _sms.sendSms(mobile, 2, cachemodel.birdhouse_name, "湿度");
                                if (status.Result) {
                                    cachemodel.sms -= 1;
                                    cachemodel.smsEnableFlag = false;
                                    CacheUntity.SetCache(arr[0], cachemodel);
                                    LogHelper.Error("湿度-短信：数据" + data + "设置" + JsonConvert.SerializeObject(cachemodel));
                                }
                            }
                            break;
                        case 30:
                            if (cachemodel.sms > 0 && cachemodel.smsEnableFlag == true) {
                                var status = _sms.sendSms(mobile, 2, cachemodel.birdhouse_name, "温度和湿度");
                                if (status.Result) {
                                    cachemodel.sms -= 1;
                                    cachemodel.smsEnableFlag = false;
                                    CacheUntity.SetCache(arr[0], cachemodel);
                                    LogHelper.Error("温度和湿度-短信" + data + "设置" + JsonConvert.SerializeObject(cachemodel));
                                }
                            }
                            break;
                        case 40:
                            //判断电话剩余数量
                            if (cachemodel.phone > 0) {
                                //如果是第三次发送进入报警
                                if (cachemodel.call_num >= 2) {
                                    LogHelper.Error("开始报警-短信");
                                   var content = "电源";
                                    //拨打电话
                                    CacheWarningModel warncache = new CacheWarningModel();
                                    warncache.mobile = mobile;
                                    warncache.mobilelist = mobileList;
                                    warncache.uncalledmobile = mobileList;
                                    warncache.uncalledmobile.Remove(mobile);
                                    warncache.calledmobile = mobileList;
                                    warncache.calledmobile.Clear();
                                    warncache.calledmobile.Add(mobile);
                                    warncache.birdhouse = cachemodel.birdhouse_name;
                                    warncache.birdhouse = content;
                                    CacheUntity.SetCache(mobile, warncache);
                                    var status = _call.templateNoticeCall(mobile, "01053924437", cachemodel.birdhouse_name, content);
                                    LogHelper.Error("时间: " + DateTime.Now.ToString() + "第1次电话通知状态" + status + "requestId：" + "被叫号码" + mobile);
                                    if (!status.Result&& mobileList.Count>1) {
                                        var mobile1 = mobileList[1];
                                        status = _call.templateNoticeCall(mobile1, "01053924437", cachemodel.birdhouse_name, content);
                                        LogHelper.Error("时间: " + DateTime.Now.ToString() + "第2次电话通知状态" + status + "requestId：" + "被叫号码" + mobile1);
                                        if (!status.Result && mobileList.Count > 2)
                                        {
                                            var mobile2 = mobileList[2];
                                            status = _call.templateNoticeCall(mobile2, "01053924437", cachemodel.birdhouse_name, content);
                                            LogHelper.Error("时间: " + DateTime.Now.ToString() + "第3次电话通知状态" + status + "requestId：" + "被叫号码" + mobile2);
                                        }
                                    }
                                    else { }
                                    LogHelper.Error("电源-电话" + data + "设置" + JsonConvert.SerializeObject(cachemodel));
                                    cachemodel.call_num = 0;
                                    CacheUntity.SetCache(arr[0], cachemodel);
                                    LogHelper.Error("时间: "+DateTime.Now.ToString()+"电话通知状态" + status + "requestId：" + "被叫号码" + mobile);
                                    cachemodel.phone -= 1;
                                } else
                                {
                                    cachemodel.call_num += 1;
                                    CacheUntity.SetCache(arr[0], cachemodel);
                                }
                            }
                            
                            break;
                        case 50:
                            if (cachemodel.phone > 0) {
                                if (cachemodel.call_num >= 2) {
                                    var content = "电源和温度";
                                    var status = _call.templateNoticeCall(mobile, "01053924437", cachemodel.birdhouse_name, content);
                                    LogHelper.Error("时间: " + DateTime.Now.ToString() + "第1次电话通知状态" + status + "requestId：" + "被叫号码" + mobile);
                                    if (!status.Result && mobileList.Count > 1)
                                    {
                                        var mobile1 = mobileList[1];
                                        status = _call.templateNoticeCall(mobile1, "01053924437", cachemodel.birdhouse_name, content);
                                        LogHelper.Error("时间: " + DateTime.Now.ToString() + "第2次电话通知状态" + status + "requestId：" + "被叫号码" + mobile1);
                                        if (!status.Result && mobileList.Count > 2)
                                        {
                                            var mobile2 = mobileList[2];
                                            status = _call.templateNoticeCall(mobile2, "01053924437", cachemodel.birdhouse_name, content);
                                            LogHelper.Error("时间: " + DateTime.Now.ToString() + "第3次电话通知状态" + status + "requestId：" + "被叫号码" + mobile2);
                                        }
                                    }
                                    LogHelper.Error("电源和温度-电话" + data + "设置" + JsonConvert.SerializeObject(cachemodel));
                                    cachemodel.call_num = 0;
                                    CacheUntity.SetCache(arr[0], cachemodel);
                                    cachemodel.phone -= 1;
                                    CacheUntity.SetCache(arr[0], cachemodel);

                                } else
                                {
                                    cachemodel.call_num += 1;
                                    CacheUntity.SetCache(arr[0], cachemodel);
                                }
                            }
                            
                            break;
                        case 60:
                            if (cachemodel.phone > 0) {
                                if (cachemodel.call_num >= 2) {
                                    var content = "电源和湿度";
                                    var status  = _call.templateNoticeCall(mobile, "01053182632", cachemodel.birdhouse_name, content);
                                    LogHelper.Error("时间: " + DateTime.Now.ToString() + "第1次电话通知状态" + status + "requestId：" + "被叫号码" + mobile);
                                    if (!status.Result && mobileList.Count > 1)
                                    {
                                        var mobile1 = mobileList[1];
                                        status = _call.templateNoticeCall(mobile1, "01053924437", cachemodel.birdhouse_name, content);
                                        LogHelper.Error("时间: " + DateTime.Now.ToString() + "第2次电话通知状态" + status + "requestId：" + "被叫号码" + mobile1);
                                        if (!status.Result && mobileList.Count > 2)
                                        {
                                            var mobile2 = mobileList[2];
                                            status = _call.templateNoticeCall(mobile2, "01053924437", cachemodel.birdhouse_name, content);
                                            LogHelper.Error("时间: " + DateTime.Now.ToString() + "第3次电话通知状态" + status + "requestId：" + "被叫号码" + mobile2);
                                        }
                                    }
                                    LogHelper.Error("电源和湿度-电话" + data + "设置" + JsonConvert.SerializeObject(cachemodel));
                                    cachemodel.call_num = 0;
                                    CacheUntity.SetCache(arr[0], cachemodel);
                                    cachemodel.phone -= 1;
                                    CacheUntity.SetCache(arr[0], cachemodel);

                                } else
                                {
                                    cachemodel.call_num += 1;
                                    CacheUntity.SetCache(arr[0], cachemodel);
                                }
                            }
                            
                            break;
                        case 70:
                            if (cachemodel.phone > 0) {
                                if (cachemodel.call_num >= 2) {
                                   var content = "电源,湿度和温度";
                                    //var status  = _call.templateNoticeCall(mobile, "01053182632", cachemodel.birdhouse_name, content);
                                    var status = _call.templateNoticeCall(mobile, "01053182632", cachemodel.birdhouse_name, content);
                                    LogHelper.Error("时间: " + DateTime.Now.ToString() + "第1次电话通知状态" + status + "requestId：" + "被叫号码" + mobile);
                                    if (!status.Result && mobileList.Count > 1)
                                    {
                                        var mobile1 = mobileList[1];
                                        status = _call.templateNoticeCall(mobile1, "01053924437", cachemodel.birdhouse_name, content);
                                        LogHelper.Error("时间: " + DateTime.Now.ToString() + "第2次电话通知状态" + status + "requestId：" + "被叫号码" + mobile1);
                                        if (!status.Result && mobileList.Count > 2)
                                        {
                                            var mobile2 = mobileList[2];
                                            status = _call.templateNoticeCall(mobile2, "01053924437", cachemodel.birdhouse_name, content);
                                            LogHelper.Error("时间: " + DateTime.Now.ToString() + "第3次电话通知状态" + status + "requestId：" + "被叫号码" + mobile2);
                                        }
                                    }
                                    LogHelper.Error("电源,湿度和温度-电话" + data + "设置" + JsonConvert.SerializeObject(cachemodel));
                                    cachemodel.call_num = 0;
                                    CacheUntity.SetCache(arr[0], cachemodel);
                                    cachemodel.phone -= 1;
                                    CacheUntity.SetCache(arr[0], cachemodel);

                                } else
                                {
                                    cachemodel.call_num += 1;
                                    CacheUntity.SetCache(arr[0], cachemodel);
                                }
                            }
                            
                            break;
                    }
                }


                /////记录
                var log = new Equipment_status_logs();
                log.humidity = arr[2];
                log.power_status = arr[3].ToInt();
                log.serial = arr[0];
                log.temperature = arr[1];
                log.create_time = Util.ToUnixStamp(DateTime.Now).ToString();
                log.update_time = Util.ToUnixStamp(DateTime.Now).ToString();
                _logsRepo.Add(log);
            }

            
            //通知websocket
            //if ($res) {
            //$connection->send('yes');
            //} else {
            //$connection->send('no');
        }

        #endregion Public Methods
    }
}