using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskDemo.BAL;
using TaskDemo.Models;

namespace TaskDemo.Controllers
{
    // [Route("[controller]")]
    public class TaskController : Controller
    {
        private readonly ILogger<TaskController> _logger;
        private readonly TaskHelper _taskHelper;

        public TaskController(ILogger<TaskController> logger, TaskHelper taskHelper)
        {
            _logger = logger;
            _taskHelper = taskHelper;
        }

        public IActionResult Index()
        {
            // Console.WriteLine(HttpContext.Session.GetString("username"));
            if (HttpContext.Session.GetString("role") == "admin")
            {
                var tasks = _taskHelper.GetAllTask();
                return View(tasks);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }


        public IActionResult Details(int id)
        {
            MyTask task = _taskHelper.GetOneTask(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }



        [HttpPost]

        public IActionResult Add(MyTask task)
        {
            task.c_taskusername = "default_username";
            _taskHelper.AddTask(task);
            return RedirectToAction("Index");

        }

        public IActionResult AddToMyTask(int id)
        {
            MyTask addtoMyTask = _taskHelper.GetOneTask(id);
            _taskHelper.AddToMyTask(addtoMyTask);
            return RedirectToAction("MyTask");
        }

        public IActionResult UpdateStatus(int id)
        {
            if (HttpContext.Session.GetString("username") != "")
            {
                MyTask updateStatus = _taskHelper.GetOneTask(id);
                _taskHelper.UpdateStatus(updateStatus);
                return RedirectToAction("MyTask");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        public IActionResult Delete(int id)
        {
            MyTask taskToDelete = _taskHelper.GetOneTask(id);
            if (taskToDelete == null)
            {
                return NotFound(); // Return 404 if task not found
            }
            _taskHelper.DeleteTask(taskToDelete);
            return RedirectToAction("Index");
        }

        public IActionResult MyTask()
        {
            if (HttpContext.Session.GetString("username") != "")
            {
                List<MyTask> taskList = _taskHelper.GetMyTask();
                return View(taskList);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public IActionResult TaskManager()
        {
            if (HttpContext.Session.GetString("username") != "")
            {
                List<MyTask> taskList = _taskHelper.GetUsersTask();
                return View(taskList);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

    }
}