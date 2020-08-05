using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yzbcore.Models
{
    public class Auth_group
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rules { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string create_operator_id { get; set; }
    }

    public partial class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public Auth_group auth_group { get; set; }
    }

}
