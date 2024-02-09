using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskDemo.Models
{
    public class Task
    {
        public int id { set; get; }

        public string name { set; get; }

        public int taskType { set; get; }

        public string todayDate { set; get; }

        public string dueDate { set; get; }
    }
}