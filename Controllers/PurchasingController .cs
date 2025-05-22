using Microsoft.AspNetCore.Mvc;
using ProMats.Function;
using ProMats.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ProMats.Controllers
{
    public class PurchasingController : Controller
    {
        private string DbConnection()
        {
            var dbAccess = new DatabaseAccessLayer();
            string dbString = dbAccess.ConnectionString;
            return dbString;
        }
        private readonly IHostingEnvironment hostingEnvironment;
        public PurchasingController(ILogger<PurchasingController> logger, IHostingEnvironment environment, DatabaseAccessLayer dbAccess)
        {
            hostingEnvironment = environment;
        }

        public IActionResult Dashboard()
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

        public IActionResult Material()
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
        public IActionResult History()
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
        public IActionResult GET_TOTAL_PURCHASING()
        {
            List<DashboardModel> data = new List<DashboardModel>();
            string query = @"SELECT COUNT(*) AS req 
                 FROM mst_purchase_request";

            using (SqlConnection conn = new SqlConnection(DbConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = conn;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data_list = new DashboardModel();
                            data_list.total = Convert.ToInt32(reader["req"].ToString());
                            data.Add(data_list);
                        }
                    }
                    conn.Close();
                }
            }

            return Json(new { items = data });
        }

        [HttpGet]
        public IActionResult GET_CLOSE_PR()
        {
            List<DashboardModel> data = new List<DashboardModel>();
            string query = @"SELECT COUNT(*) AS req 
                 FROM mst_purchase_request 
                 WHERE status = 'CLOSE'";

            using (SqlConnection conn = new SqlConnection(DbConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = conn;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data_list = new DashboardModel();
                            data_list.close = Convert.ToInt32(reader["req"].ToString());
                            data.Add(data_list);
                        }
                    }
                    conn.Close();
                }
            }

            return Json(new { items = data });
        }

        [HttpGet]
        public IActionResult GET_OPEN_PR()
        {
            List<DashboardModel> data = new List<DashboardModel>();
            string query = @"SELECT COUNT(*) AS req 
                 FROM mst_purchase_request 
                 WHERE status = 'OPEN'";

            using (SqlConnection conn = new SqlConnection(DbConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = conn;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data_list = new DashboardModel();
                            data_list.open = Convert.ToInt32(reader["req"].ToString());
                            data.Add(data_list);
                        }
                    }
                    conn.Close();
                }
            }

            return Json(new { items = data });
        }

        [HttpGet]
        public IActionResult GET_INP_PR()
        {
            List<DashboardModel> data = new List<DashboardModel>();
            string query = @"SELECT COUNT(*) AS req 
                 FROM mst_purchase_request 
                 WHERE status = 'IN-PROGRESS'";

            using (SqlConnection conn = new SqlConnection(DbConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = conn;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data_list = new DashboardModel();
                            data_list.in_progress = Convert.ToInt32(reader["req"].ToString());
                            data.Add(data_list);
                        }
                    }
                    conn.Close();
                }
            }

            return Json(new { items = data });
        }

        [HttpGet]
        public async Task<IActionResult> GET_PURCHASING_REQ(string date_from, string date_to)
        {
            var db = new DatabaseAccessLayer();
            List<PRModel> datalist = await db.GET_PURCHASING_REQ(date_from, date_to);
            return PartialView("_TablePurchasing", datalist);
        }


        [HttpGet]
        public async Task<IActionResult> GET_DASHBOARD_REQ(string date_from, string date_to)
        {
            var db = new DatabaseAccessLayer();
            List<PRModel> datalist = await db.GET_PURCHASING_REQ(date_from, date_to);
            return PartialView("_TableDash", datalist);
        }

        [HttpGet]
        public async Task<IActionResult> GET_PROSES_REQ(string date_from, string date_to)
        {
            var db = new DatabaseAccessLayer();
            List<PRModel> datalist = await db.GET_PROSES_REQ(date_from, date_to);
            return PartialView("_TableProses", datalist);
        }

        [HttpGet]
        public async Task<IActionResult> GET_HISTORY_REQ(string date_from, string date_to, string status)
        {
            var db = new DatabaseAccessLayer();
            List<PRModel> datalist = await db.GET_HISTORY_REQ(date_from, date_to, status);
            return PartialView("_TableHistory", datalist);
        }

        public IActionResult REJECT_PR(string request_id)
        {
            // Pastikan hanya role "PMC" yang dapat mengakses
            if (HttpContext.Session.GetString("role") == "PURCHASING")
            {
                int rowsAffected = 0;

                // Menggunakan blok using untuk memastikan koneksi ditutup secara otomatis
                using (SqlConnection conn = new SqlConnection(DbConnection()))
                {
                    try
                    {
                        conn.Open();

                        // Memperbaiki query SQL untuk menentukan tabel yang akan di-update
                        string query = @"UPDATE mst_purchase_request SET status = 'Rejected' WHERE request_id = @request_id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@request_id", request_id);

                        // Mengeksekusi query dan mendapatkan jumlah baris yang terpengaruh
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Menangani kesalahan jika terjadi masalah dalam query atau koneksi
                        // Anda bisa log exception ini atau menambah penanganan error yang lebih baik
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        // Menutup koneksi secara eksplisit jika diperlukan
                        conn.Close();
                    }
                }

                // Mengembalikan jumlah baris yang terpengaruh sebagai JSON
                return Json(rowsAffected);
            }
            else
            {
                // Jika role tidak sesuai, redirect ke halaman logout
                return RedirectToAction("SignOut", "Login");
            }
        }
        public IActionResult APPROVE_PR(string request_id)
        {
            if (HttpContext.Session.GetString("role") == "PURCHASING")
            {
                int rowsAffected = 0;

                using (SqlConnection conn = new SqlConnection(DbConnection()))
                {
                    try
                    {
                        conn.Open();

                        string query = @"UPDATE mst_purchase_request SET status = 'IN-PROGRESS' WHERE request_id = @request_id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@request_id", request_id);

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

                return Json(rowsAffected);
            }
            else
            {
                return RedirectToAction("SignOut", "Login");
            }
        }

        [HttpPost]
        public JsonResult VERIFY_PURCHASING(string? request_id, string? supplier, string? lead_time, string? shipment, string? total_lead)
        {
            if (HttpContext.Session.GetString("role") == "PURCHASING")
            {
                try
                {
                    string? acc_by = HttpContext.Session.GetString("name");
                    using (SqlConnection con = new SqlConnection(DbConnection()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("VERIFY_PURCHASING", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add input parameters
                            cmd.Parameters.AddWithValue("@request_id", request_id);
                            cmd.Parameters.AddWithValue("@supplier", supplier);
                            cmd.Parameters.AddWithValue("@lead_time", lead_time);
                            cmd.Parameters.AddWithValue("@shipment", shipment);
                            cmd.Parameters.AddWithValue("@total_lead", total_lead);
                            cmd.Parameters.AddWithValue("@acc_by", acc_by);

                            var returnParam = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                            returnParam.Direction = ParameterDirection.Output;

                            cmd.ExecuteNonQuery();

                            int result = (int)returnParam.Value;

                            if (result == 1)
                            {
                                return Json(new { success = true, message = "Data berhasil diverifikasi." });
                            }
                            else if (result == -1)
                            {
                                return Json(new { success = false, message = "Data sudah ada." });
                            }
                            else
                            {
                                return Json(new { success = false, message = "Terjadi kesalahan saat menambahkan data." });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Terjadi kesalahan: " + ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "Anda tidak memiliki izin." });
            }
        }

        [HttpPost]
        public IActionResult GET_DETAIL_PURCHASING(string request_id)
        {
            if (string.IsNullOrEmpty(request_id))
            {
                return BadRequest("Parameter po_id tidak boleh kosong");
            }

            var db = new DatabaseAccessLayer();
            List<PRModel> datalist = db.GET_DETAIL_PURCHASING(request_id);
            return PartialView("_TableDetailPurchasing", datalist);
        }


    }
}
