using Microsoft.AspNetCore.Mvc;
using ProMats.Function;
using ProMats.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ProMats.Controllers
{
    public class SalesController : Controller
    {
        private string DbConnection()
        {
            var dbAccess = new DatabaseAccessLayer();
            string dbString = dbAccess.ConnectionString;
            return dbString;
        }
        private readonly IHostingEnvironment hostingEnvironment;
        public SalesController(ILogger<SalesController> logger, IHostingEnvironment environment, DatabaseAccessLayer dbAccess)
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

        public IActionResult POFinish()
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
        public IActionResult GET_TOTAL_SALES()
        {
            List<DashboardModel> data = new List<DashboardModel>();
            string query = @"SELECT COUNT(*) AS req 
                 FROM mst_pofinish";

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
        public IActionResult GET_CLOSE_SALES()
        {
            List<DashboardModel> data = new List<DashboardModel>();
            string query = @"SELECT COUNT(*) AS req 
                 FROM mst_pofinish 
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
        public IActionResult GET_REJECT_SALES()
        {
            List<DashboardModel> data = new List<DashboardModel>();
            string query = @"SELECT COUNT(*) AS req 
                 FROM mst_pofinish 
                 WHERE status = 'REJECTED'";

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
                            data_list.rejected = Convert.ToInt32(reader["req"].ToString());
                            data.Add(data_list);
                        }
                    }
                    conn.Close();
                }
            }

            return Json(new { items = data });
        }

        [HttpGet]
        public IActionResult GET_INP_SALES()
        {
            List<DashboardModel> data = new List<DashboardModel>();
            string query = @"SELECT COUNT(*) AS req 
                 FROM mst_pofinish 
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


        //======================================//
        //     PO FINISH GOOD CONFIGURATION     //
        //======================================//
        [HttpGet]
        public async Task<IActionResult> GET_POFINISH_GOOD(string date_from, string date_to, string status)
        {
            var db = new DatabaseAccessLayer();
            List<POFinishModel> datalist = await db.GET_POFINISH_GOOD(date_from, date_to, status);
            return PartialView("_TableSales", datalist);
        }

        [HttpPost]
        public IActionResult GET_DETAIL_SALES(string po_id)
        {
            if (string.IsNullOrEmpty(po_id))
            {
                return BadRequest("Parameter po_id tidak boleh kosong");
            }

            var db = new DatabaseAccessLayer();
            List<DetailSalesModel> datalist = db.GET_DETAIL_SALES(po_id);
            return PartialView("_TableDetailSales", datalist);
        }

        [HttpGet]
        public IActionResult GET_SELECT_POFINISH(string pofinish)
        {
            List<KeyModel> data = new List<KeyModel>();
            string query = "SELECT DISTINCT part_code FROM mst_part WHERE part_code LIKE '%" + pofinish + "%' ORDER BY part_code ASC ";
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
                            var data_list = new KeyModel();
                            data_list.Text = reader["part_code"].ToString();
                            data_list.Id = reader["part_code"].ToString();
                            data.Add(data_list);
                        }
                    }
                    conn.Close();
                }
            }

            return Json(new { items = data });
        }

        [HttpPost]
        public async Task<JsonResult> ADD_POFINISH_GOOD(string? part_code, string? part_name, string? qty, IFormFile? sales_order)
        {
            if (HttpContext.Session.GetString("role") != "SALES")
            {
                return Json(new { success = false, message = "Anda tidak memiliki izin." });
            }

            if (sales_order == null || sales_order.Length == 0)
            {
                return Json(new { success = false, message = "File order harus diupload." });
            }

            try
            {
                string? requestBy = HttpContext.Session.GetString("name");
                if (string.IsNullOrEmpty(requestBy))
                {
                    return Json(new { success = false, message = "Nama pengguna tidak ditemukan di sesi." });
                }

                using (SqlConnection con = new SqlConnection(DbConnection()))
                {
                    con.Open();

                    // Cek dulu apakah ada status 'OPEN' untuk part_code ini
                    string checkQuery = "SELECT COUNT(1) FROM mst_pofinish WHERE part_code = @part_code AND status = 'OPEN'";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@part_code", part_code ?? (object)DBNull.Value);
                        int countOpen = (int)checkCmd.ExecuteScalar();

                        if (countOpen > 0)
                        {
                            // Jika ada status OPEN, jangan simpan file atau insert data
                            return Json(new { success = false, message = "Data tidak dapat ditambahkan karena status 'In-progress'." });
                        }
                    }

                    // Jika tidak ada data OPEN, lanjut simpan file dulu
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "File_Order");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var extension = Path.GetExtension(sales_order.FileName);
                    var datetimeNow = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var uniqueFileName = $"PO_{part_code} - {datetimeNow}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await sales_order.CopyToAsync(fileStream);
                    }

                    // Insert data ke DB setelah file berhasil disimpan
                    string insertQuery = @"
                INSERT INTO mst_pofinish (part_code, part_name, qty, request_by, status, file_order) 
                VALUES (@part_code, @part_name, @qty, @request_by, 'OPEN', @file_order);
                SELECT 1;";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                    {
                        insertCmd.Parameters.AddWithValue("@part_code", part_code ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@part_name", part_name ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@qty", qty ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@request_by", requestBy);
                        insertCmd.Parameters.AddWithValue("@file_order", uniqueFileName);

                        var result = insertCmd.ExecuteScalar();
                        if (result != null && Convert.ToInt32(result) == 1)
                        {
                            return Json(new { success = true, message = "Data berhasil ditambahkan." });
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

        [HttpPost]
        public async Task<JsonResult> UPLOAD_RECEIPT_SALES(IFormFile sales_order, string po_id, string part_code)
        {
            if (HttpContext.Session.GetString("role") != "SALES")
            {
                return Json(new { success = false, message = "You do not have permission." });
            }

            if (sales_order == null || sales_order.Length == 0)
            {
                return Json(new { success = false, message = "Please upload the receipt file." });
            }

            try
            {
                string? requestBy = HttpContext.Session.GetString("name");
                if (string.IsNullOrEmpty(requestBy))
                {
                    return Json(new { success = false, message = "User name not found in session." });
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Sales_Picture");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var extension = Path.GetExtension(sales_order.FileName);
                var datetimeNow = DateTime.Now.ToString("yyyyMMddHHmmss");
                var uniqueFileName = $"RECEIPT_{part_code}_{datetimeNow}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Jika ukuran file > 1MB, lakukan kompres dan resize dengan rasio 4:3
                if (sales_order.Length > 1 * 1024 * 1024) // > 1MB
                {

                    using (var image = await SixLabors.ImageSharp.Image.LoadAsync(sales_order.OpenReadStream()))
                    {
                        // Tentukan ukuran baru dengan rasio 4:3 berdasarkan lebar asli
                        int width = image.Width;
                        int height = (width * 3) / 4;

                        // Resize dengan cropping untuk mempertahankan rasio 4:3
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(width, height),
                            Mode = ResizeMode.Crop
                        }));

                        // Simpan dengan kualitas kompresi (misal 75)
                        var encoder = new JpegEncoder()
                        {
                            Quality = 75
                        };

                        await image.SaveAsync(filePath, encoder);
                    }
                }
                else
                {
                    // File kecil, simpan langsung
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await sales_order.CopyToAsync(fileStream);
                    }
                }

                // Update DB
                using (SqlConnection con = new SqlConnection(DbConnection()))
                {
                    con.Open();

                    string updateQuery = @"
                UPDATE mst_pofinish 
                SET receipt_proof = @file_order, status = 'CLOSE' 
                WHERE po_id = @po_id

                UPDATE mst_accpofinish 
                SET receipt_proof = @file_order, status = 'CLOSE' 
                WHERE po_id = @po_id
            ";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@file_order", uniqueFileName);
                        cmd.Parameters.AddWithValue("@po_id", po_id);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Json(new { success = true, message = "Receipt proof uploaded successfully." });
                        }
                        else
                        {
                            return Json(new { success = false, message = "No data updated. Check PO ID." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GET_PARTNAME_SALES(string part)
        {
            string query = @"
        SELECT part_name
        FROM mst_part 
        WHERE part_code = @part";

            using (SqlConnection conn = new SqlConnection(DbConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@part", part);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var data = new
                            {
                                part_name = reader["part_name"].ToString()
                            };

                            return Json(data);
                        }
                    }
                }
            }

            return Json(null);
        }

        public IActionResult DELETE_POFINISH_DATA(string po_id)
        {
            if (HttpContext.Session.GetString("role") == "SALES")
            {
                int rowsAffected = 0;

                using (SqlConnection conn = new SqlConnection(DbConnection()))
                {
                    conn.Open();
                    string query = @"DELETE FROM mst_pofinish WHERE po_id = @po_id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@po_id", po_id);

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

        //<=============================================================================================================================================================================================>//



    }
}
