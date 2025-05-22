using Microsoft.AspNetCore.Mvc;
using ProMats.Function;
using ProMats.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ProMats.Controllers
{
    public class AdminController : Controller
    {
        private string DbConnection()
        {
            var dbAccess = new DatabaseAccessLayer();
            string dbString = dbAccess.ConnectionString;
            return dbString;
        }
        private readonly IHostingEnvironment hostingEnvironment;
        public AdminController(ILogger<AdminController> logger, IHostingEnvironment environment, DatabaseAccessLayer dbAccess)
        {
            hostingEnvironment = environment;
        }

        public IActionResult AdminPanel()
        {
            if (HttpContext.Session.GetString("username") != null && HttpContext.Session.GetString("username") != "")
            {
                return View();
            }
            else
            {
                return RedirectToAction("SignOut", "Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GET_USER_DATA()
        {
            var db = new DatabaseAccessLayer();
            List<LoginModel> datalist = await db.GET_USER_DATA();
            return PartialView("_TableUser", datalist);
        }

        [HttpPost]
        public JsonResult ADD_NEW_USER(string username, string name, string password, string role)
        {
            // Verify user access level
            if (HttpContext.Session.GetString("role") == "ADMIN")
            {
                try
                {
                    Authentication hashpass = new Authentication();
                    string hashedPassword = hashpass.MD5Hash(password);

                    using (SqlConnection con = new SqlConnection(DbConnection()))
                    {
                        con.Open();

                        // Cek apakah username sudah ada
                        using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM mst_users WHERE username = @username", con))
                        {
                            checkCmd.Parameters.AddWithValue("@username", username);

                            int count = (int)checkCmd.ExecuteScalar();
                            if (count > 0)
                            {
                                return Json(new { success = false, message = "Username already exists." });
                            }
                        }

                        // Insert user baru
                        using (SqlCommand insertCmd = new SqlCommand(@"
                    INSERT INTO mst_users (username, name, password, role)
                    VALUES (@username, @name, @password, @role)", con))
                        {
                            insertCmd.Parameters.AddWithValue("@username", username);
                            insertCmd.Parameters.AddWithValue("@name", name);
                            insertCmd.Parameters.AddWithValue("@password", hashedPassword);
                            insertCmd.Parameters.AddWithValue("@role", role);

                            int rowsAffected = insertCmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                return Json(new { success = true, message = "User successfully added." });
                            }
                            else
                            {
                                return Json(new { success = false, message = "Failed to insert user." });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred: " + ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "You are not authorized." });
            }
        }

        [HttpPost]
        public JsonResult UPDATE_USER(string user_id, string username, string name, string role, string password = "")
        {
            if (HttpContext.Session.GetString("role") == "ADMIN")
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(DbConnection()))
                    {
                        con.Open();

                        string query;
                        SqlCommand cmd;

                        if (!string.IsNullOrWhiteSpace(password))
                        {
                            // Hash the password only if it is provided
                            Authentication hashpass = new Authentication();
                            string hashedPassword = hashpass.MD5Hash(password);

                            query = @"UPDATE mst_users 
                              SET name = @name, role = @role, password = @password, last_update = GETDATE()
                              WHERE user_id = @user_id AND username = @username";

                            cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@password", hashedPassword);
                        }
                        else
                        {
                            query = @"UPDATE mst_users 
                              SET name = @name, role = @role, last_update = GETDATE()
                              WHERE user_id = @user_id AND username = @username";

                            cmd = new SqlCommand(query, con);
                        }

                        cmd.Parameters.AddWithValue("@user_id", user_id);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@role", role);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                            return Json(new { success = true, message = "User successfully updated." });
                        else
                            return Json(new { success = false, message = "No data was updated." });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred: " + ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "You are not authorized." });
            }
        }


        public IActionResult DELETE_USER(string user_id)
        {
            if (HttpContext.Session.GetString("role") == "ADMIN")
            {
                int rowsAffected = 0;

                using (SqlConnection conn = new SqlConnection(DbConnection()))
                {
                    conn.Open();
                    string query = @"DELETE FROM mst_users WHERE user_id = @user_id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                    rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();
                }

                return Json(rowsAffected);
            }
            else
            {
                return RedirectToAction("SignOut", "Login");
            }
        }


    }
}
