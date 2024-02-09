using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskDemo.Models
{
    public class User
    {
        public int id { set; get; }
        public string userName { set; get; }
        public string email { set; get; }

        public string password { set; get; }
        public string role { set; get; }
        public string userType { set; get; }
    }
}