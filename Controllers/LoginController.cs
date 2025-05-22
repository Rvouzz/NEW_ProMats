using Microsoft.AspNetCore.Mvc;
using ProMats.Function;
using ProMats.Models;
using System.Data.SqlClient; // For System.Data.SqlClient

namespace ProMats.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            string? data = HttpContext.Session.GetString("username");

            return View();
        }

        public IActionResult SignOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        private string DbConnection()
        {
            var dbAccess = new DatabaseAccessLayer();
            string dbString = dbAccess.ConnectionString;
            return dbString;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public IActionResult Index(LoginModel user)
        {
            var hashpassword = new Authentication();

            if (ModelState.IsValid)
            {
                List<LoginModel> userInfo = new List<LoginModel>();
                using (SqlConnection conn = new SqlConnection(DbConnection()))
                {
                    string passwordHash = hashpassword.MD5Hash(user.password);
                    string query = "SELECT * FROM mst_users WHERE username = @username AND password = @passwordHash";
                    string update_loginID_query = @"UPDATE mst_users
                                                    SET login_id = CONVERT(varchar, GETDATE(), 120)
                                                    WHERE username = @username";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", user.username);
                    cmd.Parameters.AddWithValue("@passwordHash", passwordHash);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        ViewData["Message"] = "Login Berhasil";
                        while (reader.Read())
                        {
                            var loginUser = new LoginModel();
                            loginUser.user_id = reader["user_id"].ToString();
                            loginUser.username = reader["username"].ToString();
                            loginUser.name = reader["name"].ToString();
                            loginUser.role = reader["role"].ToString();
                            userInfo.Add(loginUser);
                            HttpContext.Session.SetString("user_id", loginUser.user_id);
                            HttpContext.Session.SetString("username", loginUser.username);
                            HttpContext.Session.SetString("name", loginUser.name);
                            HttpContext.Session.SetString("role", loginUser.role);
                        }
                        if (HttpContext.Session.GetString("role") == "PMC")
                        {
                            int rowsAffected = 0;
                            SqlCommand command = new SqlCommand(update_loginID_query, conn);
                            command.Parameters.AddWithValue("@username", user.username);
                            rowsAffected = command.ExecuteNonQuery();

                            return RedirectToAction("Dashboard", "PMC");
                        }
                        if (HttpContext.Session.GetString("role") == "PURCHASING")
                        {
                            int rowsAffected = 0;
                            SqlCommand command = new SqlCommand(update_loginID_query, conn);
                            command.Parameters.AddWithValue("@username", user.username);
                            rowsAffected = command.ExecuteNonQuery();

                            return RedirectToAction("Dashboard", "Purchasing");
                        }
                        if (HttpContext.Session.GetString("role") == "SALES")
                        {
                            int rowsAffected = 0;
                            SqlCommand command = new SqlCommand(update_loginID_query, conn);
                            command.Parameters.AddWithValue("@username", user.username);
                            rowsAffected = command.ExecuteNonQuery();

                            return RedirectToAction("Dashboard", "Sales");
                        }
                        if (HttpContext.Session.GetString("role") == "ADMIN")
                        {
                            int rowsAffected = 0;
                            SqlCommand command = new SqlCommand(update_loginID_query, conn);
                            command.Parameters.AddWithValue("@username", user.username);
                            rowsAffected = command.ExecuteNonQuery();

                            return RedirectToAction("AdminPanel", "Admin");
                        }
                    }
                    else
                    {
                        ViewData["Message"] = "User dan Password tidak terdaftar!";
                    }
                    conn.Close();
                }
            }

            return View("Index");
        }


    }
}