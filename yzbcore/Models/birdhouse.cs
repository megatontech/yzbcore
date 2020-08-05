using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yzbcore.Models
{
    public class Birdhouse
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string member_id { get; set; }
        /// <summary>
        /// 
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
        public string 最小温度 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string max_temperature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        public string min_humidity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        public string max_humidity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        public string warning_ammonia_concentration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        public string warning_negative_pressure { get; set; }
        /// <summary>
        /// 
        /// </summary>
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
        public string delete_time { get; set; }
    }

    public partial class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public Birdhouse birdhouse { get; set; }
    }

}
