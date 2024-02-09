using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskDemo.Models
{
    public class Task
    {
        public int  c_taskid { set; get; }

        public string c_tasktypeid { set; get; }

        public int c_taskissue { set; get; }

        public string c_initialdate { set; get; }

        public string c_duedate { set; get; }
        public string c_status { set; get; }

    }
}