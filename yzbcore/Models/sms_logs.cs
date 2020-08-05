using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yzbcore.Models
{
    public class Sms_logs
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
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
    }

    public partial class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public Sms_logs sms_logs { get; set; }
    }

}
