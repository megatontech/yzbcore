using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yzbcore.Models
{
    public class CacheModel
    {
        public string serial { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string bind_status { get; set; }
        public string birdhouse_name { get; set; }
        public string min_temperature { get; set; }
        public string max_temperature { get; set; }
        public string min_humidity { get; set; }
        public string max_humidity { get; set; }
        public string warning_ammonia_concentration { get; set; }
        public string warning_negative_pressure { get; set; }
        public string warning_mobile { get; set; }
        public string username { get; set; }
        public bool smsEnableFlag { get; set; }
        public int sms { get; set; }
        public int phone { get; set; }
        public int call_num { get; set; }
        public string currentTemp { get; set; }
        public string currentHumidity { get; set; }
        public string currentPower { get; set; }
        public DateTime setDate { get; set; }
    }
    public class UserCacheModel 
    {
        public string token { get; set; }
        public string id { get; set; }
        public Member member { get; set; }
        public List<CacheModel> cacheModels { get; set; }
        public List<Birdhouse> birdhouses { get; set; }
    }
    public class CacheWarningModel 
    {
        public string mobile { get; set; }
        public string birdhouse { get; set; }
        public string content { get; set; }
        public List<string> mobilelist { get; set; }
        public List<string> calledmobile { get; set; }
        public List<string> uncalledmobile { get; set; }
    }
}
