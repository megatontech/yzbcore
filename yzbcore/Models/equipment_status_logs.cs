using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yzbcore.Models
{
    public class Equipment_status_logs
    {
        /// <summary>
        /// 
        /// </summary>
        public string serial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string temperature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        public string humidity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        public int power_status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string create_time { get; set; }
        public string update_time { get; set; }
    }

    public partial class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public Equipment_status_logs equipment_status_logs { get; set; }
    }

}
