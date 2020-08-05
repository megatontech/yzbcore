using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yzbcore.Models
{
    public class Warning_logs
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
        public string birdhouse_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cause { get; set; }
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
        public Warning_logs warning_logs { get; set; }
    }

}
