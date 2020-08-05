using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yzbcore.Models
{

    public class Admin
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_disable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string create_operator_id { get; set; }
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
        public Admin admin { get; set; }
    }

}
