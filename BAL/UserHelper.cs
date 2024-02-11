using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Threading.Tasks;
using Npgsql;
using Npgsql.TypeMapping;
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
            int studentID = 0, UserType = 0;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("SELECT c_role,c_usertype,c_username, COUNT(*) FROM t_taskuser WHERE c_email= @email AND c_password= @password GROUP BY c_role,  c_usertype, c_role,c_username", conn))
                {
                    command.Parameters.Add("@email", NpgsqlDbType.Varchar).Value = user.c_email;
                    command.Parameters.Add("@password", NpgsqlDbType.Varchar).Value = user.c_password;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserType = reader.GetInt32(0);
                            role = reader.GetString(1);
                            username = reader.GetString(2);
                            rowCount = reader.GetInt32(3);

                            var session = _httpContextAccessor.HttpContext.Session;
                            session.SetInt32("userRole", UserType);
                            session.SetString("role", role);
                            session.SetString("username", username);
                            // session.SetInt32("userType", UserType);
                        }
                    }

                    // Console.WriteLine(HttpContext.Session.GetString("role"));
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



                    SendEmail(user.c_email, "🌟 New User Registration 🚀", user);
                }
                conn.Close();
            }
            return rowCount;
        }


        public void SendEmail(string toAddress, string subject, User user)
        {

            MailMessage message = new MailMessage();
            message.From = new MailAddress(toAddress);
            message.To.Add(user.c_email);
            message.Subject = subject;
            message.Body = "Dear " + user.c_username + ",\n\n" +
"I am pleased to inform you that your user account has been successfully created on our platform. Welcome to our community!\n" +
"🔐 Login Credentials:\n\n" +

"Email: " + user.c_email + ",\n" +
"Password: " + user.c_password + ",\n" +
"Please ensure to keep your password secure and do not share it with anyone. If you have any questions or need assistance, feel free to reach out to our support team at savangangani8518@gmail.com.\n\n" +

"Thank you for joining us, and we look forward to serving you!\n";

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;

            NetworkCredential networkCredential = new NetworkCredential();
            // networkCredential.UserName = fromTextBox.Text;
            string mail = "20se02ce011@ppsu.ac.in";
            networkCredential.UserName = mail.ToString();
            networkCredential.Password = "kpbc ujxg suwt hwbz";

            smtpClient.Credentials = networkCredential;

            try
            {
                smtpClient.Send(message);

            }
            catch (System.Exception ex)
            {

            }

        }
    }
}