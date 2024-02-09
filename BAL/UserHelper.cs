using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using TaskDemo.Models;

namespace TaskDemo.BAL
{
    public class UserHelper
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserHelper(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {

            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _httpContextAccessor = httpContextAccessor;
        }

        public void Register1(User user)
        {

            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                int tasktypeid = GetTaskTypeId(user.c_role, conn);
                using (var cmd = new NpgsqlCommand("INSERT INTO t_taskuser(c_username,c_email,c_password,c_usertype,c_role)VALUES(@c_username,@c_email,@c_password,@c_usertype,@c_role)", conn))
                {
                    cmd.Parameters.AddWithValue("@c_username", user.c_username);
                    cmd.Parameters.AddWithValue("@c_email", user.c_email);
                    cmd.Parameters.AddWithValue("@c_password", user.c_password);
                    cmd.Parameters.AddWithValue("@c_usertype", user.c_usertype);
                    cmd.Parameters.AddWithValue("@c_role", user.c_role);

                    cmd.ExecuteNonQuery();
                }
            }

            // var session = _httpContextAccessor.HttpContext.Session;
            // session.SetInt32("userid", user.id);
            // session.SetString("username", user.userName);
            // session.SetString("lastActivity", DateTime.UtcNow.ToString("o"));

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


        public int Login(User user)
        {
            int rowCount = 0;
            string role = "";
            string username = "";
            int studentID = 0;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("SELECT c_username,c_email,c_usertype,COUNT(*) FROM t_taskuser WHERE c_email= @email AND c_password=@password GROUP BY c_username, c_email, c_usertype", conn))
                {
                    command.Parameters.Add("@email", NpgsqlDbType.Varchar).Value = user.c_email;
                    command.Parameters.Add("@password", NpgsqlDbType.Varchar).Value = user.c_password;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            role = reader.GetString(2);
                            username = reader.GetString(0);
                            rowCount = reader.GetInt32(3);

                            var session = _httpContextAccessor.HttpContext.Session;
                            session.SetString("username", username);
                            session.SetString("role", role);
                        }
                    }
                }
                conn.Close();
            }
            return rowCount;
        }




        public int Register(User user)
        {
            int rowCount = 0;
            string username = "";
            int studentID = 0;


            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                int roleId = GetTaskTypeId(user.c_role, conn);
                Console.WriteLine("IDDDDDDDDDDDDDDDD" + roleId);
                using (var cmd = new NpgsqlCommand("INSERT INTO t_taskuser(c_username,c_email,c_password,c_usertype,c_role)VALUES(@c_username,@c_email,@c_password,@c_usertype,@c_role)", conn))
                {
                    cmd.Parameters.AddWithValue("@c_username", user.c_username);
                    cmd.Parameters.AddWithValue("@c_email", user.c_email);
                    cmd.Parameters.AddWithValue("@c_password", user.c_password);
                    cmd.Parameters.AddWithValue("@c_usertype", user.c_usertype);
                    cmd.Parameters.AddWithValue("@c_role", roleId);

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return rowCount;
        }

    }
}