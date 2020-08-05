using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
namespace yzbcore.Models
{
    public class ModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
    }
    public class AdminListViewModel: ModelBase
    {
        public AdminListViewModel() {  data = new List<Admin>(); }
        public List<Admin> data { get; set; }
    }
    public class EquipListViewModel : ModelBase
    {
        public EquipListViewModel() { data = new List<EquipmentDisp>(); }
        public List<EquipmentDisp> data { get; set; }
    }
    public class MemberListViewModel : ModelBase
    {
        public MemberListViewModel() { data = new List<Member>(); }
        public List<Member> data { get; set; }
    }
}
