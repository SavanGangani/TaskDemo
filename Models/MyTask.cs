using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskDemo.Models
{
    public class MyTask
    {
        public int  c_taskid { set; get; }

        public string c_tasktypeid { set; get; }
        public int c_tasktid { set; get; }
        public string c_tasktype {set; get;}

        public string c_taskissue { set; get; }

        public DateTime c_initialdate { set; get; }

        public DateTime c_duedate { set; get; }
        public string c_status { set; get; }

        public string c_taskusername {set; get;}
    }
}