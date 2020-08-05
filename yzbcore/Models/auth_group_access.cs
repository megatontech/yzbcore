using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yzbcore.Models
{

    public class Auth_group_access
    {
        /// <summary>
        /// 
        /// </summary>
        public string uid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string group_id { get; set; }
}

public partial class Root
{
    /// <summary>
    /// 
    /// </summary>
    public Auth_group_access auth_group_access { get; set; }
}

}
