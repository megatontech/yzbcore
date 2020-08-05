using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yzbcore.Models
{
    public class Equipment
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string serial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string birdhouse_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string member_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bind_status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
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
    public class EquipmentDisp
    {

        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string serial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bind_status { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string create_time { get; set; }
       
    }

    public partial class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public Equipment equipment { get; set; }
    }

}
