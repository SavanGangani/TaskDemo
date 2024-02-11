using System;
using System.Collections.Generic;
using System.Linq;
using TaskDemo.Models;
using Npgsql;
using Microsoft.AspNetCore.Http;

namespace TaskDemo.BAL
{
    public class TaskHelper
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskHelper(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _httpContextAccessor = httpContextAccessor;

        }

        public List<MyTask> GetAllTask()
        {
            List<MyTask> taskList = new List<MyTask>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM t_task ,t_tasktype  WHERE t_task.c_tasktypeid = t_tasktype.c_tasktypeid ORDER BY c_taskid", conn))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var task = new MyTask
                        {
                            c_taskid = Convert.ToInt32(dr["c_taskid"]),
                            c_tasktypeid = dr["c_tasktype"].ToString(),
                            c_taskissue = dr["c_taskissue"].ToString(),
                            c_initialdate = DateTime.Parse(dr["c_initialdate"].ToString()),
                            c_duedate = DateTime.Parse(dr["c_duedate"].ToString()),
                            c_status = dr["c_status"].ToString(),
                            c_taskusername = dr["c_taskusername"].ToString(),
                        };

                        taskList.Add(task);
                    }
                }
            }
            return taskList;
        }


        public MyTask GetOneTask(int id)
        {
            var task = new MyTask();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM t_task INNER JOIN t_tasktype ON t_task.c_tasktypeid = t_tasktype.c_tasktypeid WHERE t_task.c_taskid = @c_taskid", conn))
                {
                    cmd.Parameters.AddWithValue("@c_taskid", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            task.c_taskid = Convert.ToInt32(dr["c_taskid"]);
                            task.c_tasktypeid = dr["c_tasktype"].ToString();
                            task.c_taskissue = dr["c_taskissue"].ToString();
                            task.c_initialdate = DateTime.Parse(dr["c_initialdate"].ToString());
                            task.c_duedate = DateTime.Parse(dr["c_duedate"].ToString());
                            task.c_status = dr["c_status"].ToString();
                            task.c_taskusername = dr["c_taskusername"].ToString();
                        }
                    }
                }
            }
            return task;
        }

        public void AddTask(MyTask task)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                int tasktypeid = GetTaskTypeId(task.c_tasktypeid, conn);

                using (var cmd = new NpgsqlCommand("INSERT INTO t_task(c_tasktypeid,c_taskissue,c_initialdate,c_duedate,c_status,c_taskusername) VALUES (@c_tasktypeid, @c_taskissue, @c_initialdate, @c_duedate, @c_status,@c_taskusername)", conn))
                {

                    cmd.Parameters.AddWithValue("@c_tasktypeid", tasktypeid);
                    cmd.Parameters.AddWithValue("@c_taskissue", task.c_taskissue);
                    cmd.Parameters.AddWithValue("@c_initialdate", task.c_initialdate);
                    cmd.Parameters.AddWithValue("@c_duedate", task.c_duedate);
                    cmd.Parameters.AddWithValue("@c_status", task.c_status);
                    cmd.Parameters.AddWithValue("@c_taskusername", task.c_taskusername);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddToMyTask(MyTask myTask)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var session = _httpContextAccessor.HttpContext.Session;
                myTask.c_taskusername = session.GetString("username");

                using (var cmd = new NpgsqlCommand("UPDATE t_task SET c_taskusername = @user WHERE c_taskid = @taskid", conn))
                {
                    cmd.Parameters.AddWithValue("@user", myTask.c_taskusername);
                    cmd.Parameters.AddWithValue("@taskid", myTask.c_taskid);

                    cmd.ExecuteNonQuery();
                }
            }
        }

         public void UpdateStatus(MyTask myTask)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("UPDATE t_task SET c_status = 'Done'  WHERE c_taskid = @taskid", conn))
                {
                    cmd.Parameters.AddWithValue("@taskid", myTask.c_taskid);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // public List<MyTask> GetMyTask()
        // {
        //     List<MyTask> taskList = new List<MyTask>();

        //     using (var conn = new NpgsqlConnection(_connectionString))
        //     {
        //         conn.Open();


        //         using (var cmd = new NpgsqlCommand("SELECT * FROM t_task where c_tasktypeid = @c_tasktypeid ", conn))
        //         {
        //             // var session = _httpContextAccessor.HttpContext.Session;
        //             // var userId = session.GetInt32("userRole");
        //             // Console.WriteLine("UUUSERRDIDID: " + userId);
        //             // int userType = HttpContext.Session.Try("userType")
        //             // ?? 0; //s Default value if not found

        //             cmd.Parameters.AddWithValue("@c_tasktypeid", 2);

        //             using (var dr = cmd.ExecuteReader())
        //             {
        //                 while (dr.Read())
        //                 {
        //                     var stu = new MyTask
        //                     {
        //                         c_taskid = Convert.ToInt32(dr["c_taskid"]),
        //                         c_taskissue = dr["c_taskissue"].ToString(),
        //                         c_tasktypeid = dr["c_tasktypeid"].ToString(),
        //                         c_initialdate = DateTime.Parse(dr["c_initialdate"].ToString()),
        //                         c_duedate = DateTime.Parse(dr["c_duedate"].ToString()),
        //                         c_status = dr["c_status"].ToString(),

        //                     };

        //                     taskList.Add(stu);
        //                 }
        //             }
        //         }



        //     }
        //     return taskList;
        // }

        public List<MyTask> GetMyTask()
        {
            List<MyTask> taskList = new List<MyTask>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM t_task where c_taskusername = @username ", conn))
                {
                    var session = _httpContextAccessor.HttpContext.Session;
                    var userName = session.GetString("username");
                    Console.WriteLine(userName);
                    cmd.Parameters.AddWithValue("@username", userName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var stu = new MyTask
                            {
                                c_taskid = Convert.ToInt32(dr["c_taskid"]),
                                c_taskissue = dr["c_taskissue"].ToString(),
                                c_tasktypeid = dr["c_tasktypeid"].ToString(),
                                c_initialdate = DateTime.Parse(dr["c_initialdate"].ToString()),
                                c_duedate = DateTime.Parse(dr["c_duedate"].ToString()),
                                c_status = dr["c_status"].ToString(),

                            };

                            taskList.Add(stu);
                        }
                    }
                }



            }
            return taskList;
        }

        public List<MyTask> GetUsersTask()
        {
            List<MyTask> taskList = new List<MyTask>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM t_task where c_tasktypeid = @c_tasktypeid AND c_status = 'Pending' AND c_taskusername = 'default_username'", conn))
                {
                    var session = _httpContextAccessor.HttpContext.Session;
                    var userId = session.GetInt32("userRole");

                    cmd.Parameters.AddWithValue("@c_tasktypeid", userId);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var stu = new MyTask
                            {
                                c_taskid = Convert.ToInt32(dr["c_taskid"]),
                                c_taskissue = dr["c_taskissue"].ToString(),
                                c_tasktypeid = dr["c_tasktypeid"].ToString(),
                                c_initialdate = DateTime.Parse(dr["c_initialdate"].ToString()),
                                c_duedate = DateTime.Parse(dr["c_duedate"].ToString()),
                                c_status = dr["c_status"].ToString(),

                            };

                            taskList.Add(stu);
                        }
                    }
                }



            }
            return taskList;
        }

        private int GetTaskTypeId(string TaskType, NpgsqlConnection conn)
        {
            int tasktypeid = 0;

            using (var cmd = new NpgsqlCommand("SELECT c_tasktypeid FROM t_tasktype WHERE c_tasktype = @tasktype", conn))
            {
                cmd.Parameters.AddWithValue("@tasktype", TaskType);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tasktypeid = reader.GetInt32(0);
                    }
                }
            }

            return tasktypeid;
        }

        public void EditTask(MyTask task)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                int tasktypeid = GetTaskTypeId(task.c_tasktypeid, conn);
                using (var cmd = new NpgsqlCommand("UPDATE t_task SET c_tasktypeid=@c_tasktypeid , c_taskissue=@c_taskissue , c_initialdate=@c_initialdate , c_duedate=@c_duedate , c_status=@c_status WHERE c_taskid =@c_taskid ", conn))
                {
                    cmd.Parameters.AddWithValue("@c_taskid", task.c_taskid);
                    cmd.Parameters.AddWithValue("@c_tasktypeid", tasktypeid);
                    cmd.Parameters.AddWithValue("@c_taskissue", task.c_taskissue);
                    cmd.Parameters.AddWithValue("@c_initialdate", task.c_initialdate);
                    cmd.Parameters.AddWithValue("@c_duedate", task.c_duedate);
                    cmd.Parameters.AddWithValue("@c_status", task.c_status);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTask(MyTask task)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM t_task WHERE c_taskid= @c_taskid", conn))
                {
                    cmd.Parameters.AddWithValue("@c_taskid", task.c_taskid);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}