using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class PMCController : Controller
    {
        private string DbConnection()
        {
            var dbAccess = new DatabaseAccessLayer();
            string dbString = dbAccess.ConnectionString;
            return dbString;
        }
        private readonly IHostingEnvironment hostingEnvironment;
        public PMCController(ILogger<PMCController> logger, IHostingEnvironment environment, DatabaseAccessLayer dbAccess)
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

        public IActionResult Purchase()
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

        public IActionResult PartList()
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

        public IActionResult POFinishPMC()
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

        public IActionResult FGTracking()
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


        //===============================//
        //    DASHBOARD CONFIGURATION    //
        //===============================//
        [HttpGet]
        public async Task<IActionResult> GET_DASHBOARD_PART()
        {
            var db = new DatabaseAccessLayer();
            List<DashboardModel> datalist = await db.GET_DASHBOARD_PART();
            return PartialView("_TableDashboard", datalist);
        }

        [HttpGet]
        public IActionResult GET_SHORTAGE_QTY()
        {
            List<DashboardModel> data = new List<DashboardModel>();
            string query = @"
        SELECT COUNT(*) AS req
        FROM (
            SELECT 
                m.id_mtl,
                m.material_id,
                m.material_name,
                m.opening_stock,
                m.incoming,
                ISNULL(SUM(a.shot), 0) AS total_req,
                m.part_code
            FROM 
                [DB_PROMATS].[dbo].[mst_material] AS m
            LEFT JOIN 
                [DB_PROMATS].[dbo].[mst_accpofinish] AS a
            ON 
                m.part_code = a.part_code 
                AND a.status = 'IN-PROGRESS'
            GROUP BY 
                m.id_mtl,
                m.material_id,
                m.material_name,
                m.opening_stock,
                m.incoming,
                m.part_code
            HAVING 
                (m.opening_stock + m.incoming - ISNULL(SUM(a.shot), 0)) < 0
        ) AS shortage_materials;
    ";

            using (SqlConnection conn = new SqlConnection(DbConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data_list = new DashboardModel();
                            data_list.total_shortage = Convert.ToInt32(reader["req"].ToString());
                            data.Add(data_list);
                        }
                    }
                }
            }

            return Json(new { items = data });
        }


        [HttpGet]
        public IActionResult GET_APPROVED_REQ()
        {
            List<DashboardModel> data = new List<DashboardModel>();
            string query = @"SELECT COUNT(*) AS req 
                 FROM mst_pofinish
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
        public IActionResult GET_WAITING_REQ()
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
        public IActionResult GET_REJECTED_REQ()
        {
            List<DashboardModel> data = new List<DashboardModel>();
            string query = @"SELECT COUNT(*) AS req 
                 FROM mst_accpofinish 
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
        public IActionResult GET_DONUT_PMC(string date_from, string date_to)
        {
            var data = new List<DashboardModel>();
            using (SqlConnection conn = new SqlConnection(DbConnection()))
            {
                var command = new SqlCommand("GET_DONUT_PMC", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@date_from", date_from);
                command.Parameters.AddWithValue("@date_to", date_to);
                conn.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new DashboardModel
                        {
                            waiting = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            approved = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                            rejected = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                            in_progress = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                        });
                    }
                }
            }
            Console.WriteLine(JsonConvert.SerializeObject(data));
            return Json(data);
        }

        [HttpGet]
        public IActionResult GET_YEARLY_PMC()
        {
            var data = new List<DashboardModel>();
            using (SqlConnection conn = new SqlConnection(DbConnection()))
            {
                var command = new SqlCommand("GET_YEARLY_PMC", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new DashboardModel
                        {
                            month = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            count = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                        });
                    }
                }
            }
            Console.WriteLine(JsonConvert.SerializeObject(data));
            return Json(data);
        }

        //<=============================================================================================================================================================================================>//



        //===============================//
        //    PART DATA CONFIGURATION    //
        //===============================//
        [HttpGet]
        public async Task<IActionResult> GET_MASTER_PART()
        {
            var db = new DatabaseAccessLayer();
            List<MstPartModel> datalist = await db.GET_MASTER_PART();
            return PartialView("_TableMasterPart", datalist);
        }

        [HttpGet]
        public async Task<IActionResult> GET_MASTER_MATERIAL(string pcode)
        {
            var db = new DatabaseAccessLayer();
            List<PartModel> datalist = await db.GET_MASTER_MATERIAL(pcode);
            return PartialView("_TableMaterial", datalist);
        }

        [HttpGet]
        public IActionResult GET_SELECT_PART_CODE(string pcode)
        {
            List<KeyModel> data = new List<KeyModel>();
            string query = "SELECT DISTINCT part_code FROM mst_material WHERE part_code LIKE '%" + pcode + "%' ORDER BY part_code ASC ";
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

        [HttpGet]
        public IActionResult GET_SELECT_PCODE(string pcode)
        {
            List<KeyModel> data = new List<KeyModel>();
            string query = "SELECT DISTINCT part_code FROM mst_part WHERE part_code LIKE '%" + pcode + "%' ORDER BY part_code ASC ";
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

        public IActionResult DOWNLOAD_MASTER_PART()
        {
            // Define the current date and time in the desired format
            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("yyyy.MM.dd - HH:mm:ss");

            // Retrieve master part data
            DataSet ds = GET_MASTER_PARTDATA();
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return BadRequest("No data available for download.");
            }

            DataTable dt = ds.Tables[0];

            // Create Excel file using ClosedXML
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Master Part List");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    // Return the file as a download
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"MASTER_PART_LIST_{formattedDateTime}.xlsx"
                    );
                }
            }
        }

        private DataSet GET_MASTER_PARTDATA()
        {
            DataSet ds = new DataSet();

            try
            {
                // Connect to the database
                using (SqlConnection conn = new(DbConnection()))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    [material_id],
                    [material_name],
                    [opening_stock],
                    [incoming],
                    [total_req],
                    [opening_stock] + [incoming] - [total_req] AS shortage_qty
                FROM mst_part";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(ds);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., logging)
                throw new Exception("Error retrieving master part data", ex);
            }

            return ds;
        }

        [HttpPost]
        public JsonResult ADD_PART_DATA(string? part_code, string? part_name, string? part, string? runner)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(DbConnection()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("ADD_PART_DATA", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add input parameters
                            cmd.Parameters.AddWithValue("@part_code", part_code);
                            cmd.Parameters.AddWithValue("@part_name", part_name);
                            cmd.Parameters.AddWithValue("@part", part);
                            cmd.Parameters.AddWithValue("@runner", runner);

                            // Add the output parameter
                            var returnParam = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                            returnParam.Direction = ParameterDirection.Output;

                            // Execute the command
                            cmd.ExecuteNonQuery();

                            // Retrieve the output parameter value
                            int result = (int)returnParam.Value;

                            // Check the result and return appropriate message
                            if (result == 1)
                            {
                                return Json(new { success = true, message = "Data berhasil ditambahkan." });
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
        public JsonResult UPDATE_PART_DATA(string? id_part, string? part_code, string? part_name, string? part, string? runner)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(DbConnection()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE_PART_DATA", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add input parameters
                            cmd.Parameters.AddWithValue("@part_code", part_code);
                            cmd.Parameters.AddWithValue("@part_name", part_name);
                            cmd.Parameters.AddWithValue("@part", part);
                            cmd.Parameters.AddWithValue("@runner", runner);
                            cmd.Parameters.AddWithValue("@id_part", id_part);

                            // Add the output parameter
                            var returnParam = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                            returnParam.Direction = ParameterDirection.Output;

                            // Execute the command
                            cmd.ExecuteNonQuery();

                            // Retrieve the output parameter value
                            int result = (int)returnParam.Value;

                            // Check the result and return appropriate message
                            if (result == 1)
                            {
                                return Json(new { success = true, message = "Data berhasil diupdate." });
                            }
                            else if (result == -1)
                            {
                                return Json(new { success = false, message = "Data sudah ada." });
                            }
                            else
                            {
                                return Json(new { success = false, message = "Terjadi kesalahan saat mengupdate data." });
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
        public IActionResult DELETE_PART_DATA(string id_part)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                int rowsAffected = 0;

                using (SqlConnection conn = new SqlConnection(DbConnection()))
                {
                    conn.Open();

                    // Begin a transaction to ensure both deletions happen together
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Delete from mst_material where part_code matches the id_part
                            string deleteMaterialQuery = @"DELETE FROM mst_material WHERE part_code = 
                                                   (SELECT part_code FROM mst_part WHERE id_part = @id_part)";
                            SqlCommand cmdMaterial = new SqlCommand(deleteMaterialQuery, conn, transaction);
                            cmdMaterial.Parameters.AddWithValue("@id_part", id_part);
                            rowsAffected += cmdMaterial.ExecuteNonQuery();

                            // Delete from mst_part
                            string deletePartQuery = @"DELETE FROM mst_part WHERE id_part = @id_part";
                            SqlCommand cmdPart = new SqlCommand(deletePartQuery, conn, transaction);
                            cmdPart.Parameters.AddWithValue("@id_part", id_part);
                            rowsAffected += cmdPart.ExecuteNonQuery();

                            // Commit the transaction
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            // In case of an error, roll back the transaction
                            transaction.Rollback();
                            return Json(new { success = false, message = "An error occurred while deleting the data." });
                        }
                    }

                    conn.Close();
                }

                return Json(new { success = true, rowsAffected });
            }
            else
            {
                return RedirectToAction("SignOut", "Login");
            }
        }


        [HttpPost]
        [Route("DELETE_SELECT_PART")]
        public IActionResult DELETE_SELECT_PART([FromBody] PartModel[] input)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                int totalDeleted = 0;

                using (SqlConnection conn = new SqlConnection(DbConnection()))
                {
                    conn.Open();
                    string query = "DELETE FROM mst_part WHERE material_id = @material_id";

                    foreach (var item in input)
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@material_id", item.material_id));
                            try
                            {
                                totalDeleted += cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Console.Error.WriteLine(ex.Message);
                                // Return an error response if necessary  
                            }
                        }
                    }
                }

                return Json(totalDeleted); // Return the total count of deleted rows  
            }
            else
            {
                return RedirectToAction("SignOut", "Login");
            }
        }

        [HttpPost]
        public JsonResult ADD_MATERIAL_DATA(string? part_code, string? material_id, string? material_name, string? opening_stock, string? incoming)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(DbConnection()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("ADD_MATERIAL_DATA", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add input parameters
                            cmd.Parameters.AddWithValue("@part_code", part_code);
                            cmd.Parameters.AddWithValue("@material_id", material_id);
                            cmd.Parameters.AddWithValue("@material_name", material_name);
                            cmd.Parameters.AddWithValue("@opening_stock", opening_stock);
                            cmd.Parameters.AddWithValue("@incoming", incoming);

                            // Add the output parameter
                            var returnParam = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                            returnParam.Direction = ParameterDirection.Output;

                            // Execute the command
                            cmd.ExecuteNonQuery();

                            // Retrieve the output parameter value
                            int result = (int)returnParam.Value;

                            // Check the result and return appropriate message
                            if (result == 1)
                            {
                                return Json(new { success = true, message = "Data berhasil ditambahkan." });
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
        public JsonResult UPDATE_MATERIAL_DATA(string? id_mtl, string? opening_stock, string? incoming)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(DbConnection()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE_MATERIAL_DATA", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add input parameters
                            cmd.Parameters.AddWithValue("@id_mtl", id_mtl);
                            cmd.Parameters.AddWithValue("@opening_stock", opening_stock);
                            cmd.Parameters.AddWithValue("@incoming", incoming);

                            // Add the output parameter
                            var returnParam = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                            returnParam.Direction = ParameterDirection.Output;

                            // Execute the command
                            cmd.ExecuteNonQuery();

                            // Retrieve the output parameter value
                            int result = (int)returnParam.Value;

                            // Check the result and return appropriate message
                            if (result == 1)
                            {
                                return Json(new { success = true, message = "Data berhasil diupdate." });
                            }
                            else if (result == -1)
                            {
                                return Json(new { success = false, message = "Data sudah ada." });
                            }
                            else
                            {
                                return Json(new { success = false, message = "Terjadi kesalahan saat mengupdate data." });
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
        public IActionResult DELETE_MATERIAL_DATA(string id_mtl)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                int rowsAffected = 0;

                using (SqlConnection conn = new SqlConnection(DbConnection()))
                {
                    conn.Open();
                    string query = @"DELETE FROM mst_material WHERE id_mtl = @id_mtl";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_mtl", id_mtl);

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



        //======================================//
        //    PURCHASE REQUEST CONFIGURATION    //
        //======================================//
        [HttpGet]
        public async Task<IActionResult> GET_PURCHASE_REQUEST(string date_from, string date_to, string status)
        {
            var db = new DatabaseAccessLayer();
            List<PRModel> datalist = await db.GET_PURCHASE_REQUEST(date_from, date_to, status);
            return PartialView("_TablePurchase", datalist);
        }

        [HttpPost]
        public IActionResult GET_DETAIL_POM(string request_id)
        {
            if (string.IsNullOrEmpty(request_id))
            {
                return BadRequest("Parameter po_id tidak boleh kosong");
            }

            var db = new DatabaseAccessLayer();
            List<PRModel> datalist = db.GET_DETAIL_PURCHASING(request_id);
            return PartialView("_TableDetailPurchasing", datalist);
        }

        [HttpGet]
        public IActionResult GET_PCODE_PR(string pcode)
        {
            List<KeyModel> data = new List<KeyModel>();
            string query = "SELECT DISTINCT part_code FROM mst_material WHERE part_code LIKE '%" + pcode + "%' ORDER BY part_code ASC ";
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

        [HttpGet]
        public IActionResult GET_MATERIAL_PR(string pcode, string mtl_id)
        {
            List<KeyModel> data = new List<KeyModel>();
            string query = "SELECT DISTINCT material_id FROM mst_material WHERE material_id LIKE @mtl_id AND part_code = @pcode ORDER BY material_id ASC";
            using (SqlConnection conn = new SqlConnection(DbConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@mtl_id", "%" + mtl_id + "%");
                    cmd.Parameters.AddWithValue("@pcode", pcode);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data_list = new KeyModel
                            {
                                Text = reader["material_id"].ToString(),
                                Id = reader["material_id"].ToString()
                            };
                            data.Add(data_list);
                        }
                    }
                }
            }

            return Json(new { items = data });
        }

        [HttpGet]
        public IActionResult GET_MATERIAL_NAME_PR(string mtl_id)
        {
            string query = @"
    SELECT 
        material_name
    FROM 
        mst_material 
    WHERE 
        material_id = @mtl_id OR @mtl_id IS NULL";

            using (SqlConnection conn = new SqlConnection(DbConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (mtl_id == null)
                    {
                        cmd.Parameters.AddWithValue("@mtl_id", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@mtl_id", mtl_id);
                    }

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var data = new
                            {
                                material_name = reader["material_name"].ToString()
                            };

                            return Json(data);
                        }
                    }
                }
            }

            return Json(null);
        }

        [HttpPost]
        public JsonResult ADD_PURCHASE_REQ(string? part_code, string? material_id, string? quantity)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                try
                {
                    string? requestBy = HttpContext.Session.GetString("name");
                    using (SqlConnection con = new SqlConnection(DbConnection()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("ADD_PURCHASE_REQ", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add input parameters
                            cmd.Parameters.AddWithValue("@part_code", part_code);
                            cmd.Parameters.AddWithValue("@material_id", material_id);
                            cmd.Parameters.AddWithValue("@quantity", quantity);
                            cmd.Parameters.AddWithValue("@request_by", requestBy);

                            // Add the output parameter
                            var returnParam = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                            returnParam.Direction = ParameterDirection.Output;

                            // Execute the command
                            cmd.ExecuteNonQuery();

                            // Retrieve the output parameter value
                            int result = (int)returnParam.Value;

                            // Check the result and return appropriate message
                            if (result == 1)
                            {
                                return Json(new { success = true, message = "Data berhasil ditambahkan." });
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
        public JsonResult UPDATE_PR_PMC(string? request_id, string? part_code, string? material_id, string? quantity)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(DbConnection()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE_PR_PMC", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add input parameters
                            cmd.Parameters.AddWithValue("@request_id", request_id);
                            cmd.Parameters.AddWithValue("@part_code", part_code);
                            cmd.Parameters.AddWithValue("@material_id", material_id);
                            cmd.Parameters.AddWithValue("@quantity", quantity);

                            // Add the output parameter
                            var returnParam = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                            returnParam.Direction = ParameterDirection.Output;

                            // Execute the command
                            cmd.ExecuteNonQuery();

                            // Retrieve the output parameter value
                            int result = (int)returnParam.Value;

                            // Check the result and return appropriate message
                            if (result == 1)
                            {
                                return Json(new { success = true, message = "Data berhasil diupdate." });
                            }
                            else if (result == -1)
                            {
                                return Json(new { success = false, message = "Data sudah ada." });
                            }
                            else
                            {
                                return Json(new { success = false, message = "Terjadi kesalahan saat mengupdate data." });
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
        public IActionResult DELETE_PR_DATA(string request_id)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                int rowsAffected = 0;

                using (SqlConnection conn = new SqlConnection(DbConnection()))
                {
                    conn.Open();
                    string query = @"DELETE FROM mst_purchase_request WHERE request_id = @request_id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@request_id", request_id);

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



        //======================================//
        //      PO FINISH CONFIGURATION         //
        //======================================//
        [HttpGet]
        public async Task<IActionResult> GET_POFINISHPMC(string date_from, string date_to)
        {
            var db = new DatabaseAccessLayer();
            List<POFinishModel> datalist = await db.GET_POFINISHPMC(date_from, date_to);
            return PartialView("_TableSales", datalist);
        }

        [HttpGet]
        public async Task<IActionResult> GET_POFINISH_ACC(string date_from, string date_to)
        {
            var db = new DatabaseAccessLayer();
            List<POAccModel> datalist = await db.GET_POFINISH_ACC(date_from, date_to);
            return PartialView("_TableAccPO", datalist);
        }

        [HttpGet]
        public IActionResult GET_RUNNER_PMC(string part_code)
        {
            string query = @"
        SELECT part, runner
        FROM mst_part 
        WHERE part_code = @part_code";

            using (SqlConnection conn = new SqlConnection(DbConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@part_code", part_code);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var data = new
                            {
                                part = reader["part"].ToString(),
                                runner = reader["runner"].ToString()
                            };

                            return Json(data);
                        }
                    }
                }
            }

            return Json(null);
        }

        [HttpPost]
        public JsonResult ACC_POFINISH_PMC(string? po_id, string? part_code, string? part_name,
                                    string? qty, string? date, string? request_by,
                                    string? part, string? runner, string? shot, string file_order)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(DbConnection()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("ACC_POFINISH_PMC", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add input parameters
                            cmd.Parameters.AddWithValue("@po_id", po_id);
                            cmd.Parameters.AddWithValue("@part_code", part_code);
                            cmd.Parameters.AddWithValue("@part_name", part_name);
                            cmd.Parameters.AddWithValue("@qty", qty);
                            cmd.Parameters.AddWithValue("@date", date);
                            cmd.Parameters.AddWithValue("@request_by", request_by);
                            cmd.Parameters.AddWithValue("@part", part);
                            cmd.Parameters.AddWithValue("@runner", runner);
                            cmd.Parameters.AddWithValue("@shot", shot);
                            cmd.Parameters.AddWithValue("@file_order", file_order);

                            // Add the output parameter
                            var returnParam = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                            returnParam.Direction = ParameterDirection.Output;

                            // Execute the command
                            cmd.ExecuteNonQuery();

                            // Retrieve the output parameter value
                            int result = (int)returnParam.Value;

                            // Check the result and return appropriate message
                            if (result == 1)
                            {
                                return Json(new { success = true, message = "Data berhasil ditambahkan." });
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

        public IActionResult REJECT_POFINISH_PMC(string po_id)
        {
            // Pastikan hanya role "PMC" yang dapat mengakses
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                int rowsAffected = 0;

                // Menggunakan blok using untuk memastikan koneksi ditutup secara otomatis
                using (SqlConnection conn = new SqlConnection(DbConnection()))
                {
                    try
                    {
                        conn.Open();

                        // Memperbaiki query SQL untuk menentukan tabel yang akan di-update
                        string query = @"UPDATE mst_pofinish SET status = 'REJECTED' WHERE po_id = @po_id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@po_id", po_id);

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

        [HttpPost]
        public JsonResult DELIVER_POFINISH_PMC(string? po_id, string? deliver_by, string? remark)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(DbConnection()))
                    {
                        con.Open();

                        // Query untuk INSERT ke tbl_detail_finish dan update status
                        string query = @"
                    BEGIN TRANSACTION;

                    -- Insert data ke tbl_detail_finish
                    INSERT INTO [DB_PROMATS].[dbo].[tbl_detail_finish] ([po_id], [deliver_by], [remark])
                    VALUES (@po_id, @deliver_by, @remark);

                    -- Update status pada mst_accpofinish
                    UPDATE [DB_PROMATS].[dbo].[mst_accpofinish]
                    SET [status] = 'SHIPPING'
                    WHERE [po_id] = @po_id;
                    
                    DECLARE @total_stock INT, @part_code VARCHAR(100), @qty INT;

                    SELECT @qty = qty, @part_code = part_code
                    FROM [mst_accpofinish]
                    WHERE po_id = @po_id;

                    SELECT @total_stock = total_stock
                    FROM [tbl_stock_pofinish]
                    WHERE part_code = @part_code;

                    UPDATE [DB_PROMATS].[dbo].[tbl_stock_pofinish]
                    SET total_stock = @total_stock - @qty
                    WHERE part_code = @part_code;

                    -- Update status pada mst_pofinish
                    UPDATE [DB_PROMATS].[dbo].[mst_pofinish]
                    SET [status] = 'SHIPPING'
                    WHERE [po_id] = @po_id;

                    COMMIT TRANSACTION;";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            // Add input parameters
                            cmd.Parameters.AddWithValue("@po_id", po_id);
                            cmd.Parameters.AddWithValue("@deliver_by", deliver_by);
                            cmd.Parameters.AddWithValue("@remark", string.IsNullOrWhiteSpace(remark) ? DBNull.Value : remark);

                            // Execute the query
                            int rowsAffected = cmd.ExecuteNonQuery();

                            // Check the result and return appropriate message
                            if (rowsAffected > 0)
                            {
                                return Json(new { success = true, message = "Data berhasil ditambahkan dan status berhasil diubah menjadi CLOSE." });
                            }
                            else
                            {
                                return Json(new { success = false, message = "Tidak ada data yang ditambahkan atau status tidak berhasil diubah." });
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




        //<=============================================================================================================================================================================================>//

        [HttpGet]
        public async Task<IActionResult> GET_HISTORY_PMC(string date_from, string date_to, string status)
        {
            var db = new DatabaseAccessLayer();
            List<POFinishModel> datalist = await db.GET_HISTORY_PMC(date_from, date_to, status);
            return PartialView("_TableHistory", datalist);
        }

        [HttpGet]
        public async Task<IActionResult> GET_FG_TRACKING()
        {
            var db = new DatabaseAccessLayer();
            List<FGTrackingModel> datalist = await db.GET_FG_TRACKING();
            return PartialView("_TableFGTracking", datalist);
        }

        [HttpPost]
        public JsonResult UPDATE_STOCK_POFINISH(string? id_det, string? part_code, string? total_stock)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(DbConnection()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE_STOCK_POFINISH", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add input parameters
                            cmd.Parameters.AddWithValue("@id_det", id_det);
                            cmd.Parameters.AddWithValue("@part_code", part_code);
                            cmd.Parameters.AddWithValue("@total_stock", total_stock);

                            // Add the output parameter
                            var returnParam = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                            returnParam.Direction = ParameterDirection.Output;

                            // Execute the command
                            cmd.ExecuteNonQuery();

                            // Retrieve the output parameter value
                            int result = (int)returnParam.Value;

                            // Check the result and return appropriate message
                            if (result == 1)
                            {
                                return Json(new { success = true, message = "Data berhasil diupdate." });
                            }
                            else if (result == -1)
                            {
                                return Json(new { success = false, message = "Data sudah ada." });
                            }
                            else
                            {
                                return Json(new { success = false, message = "Terjadi kesalahan saat mengupdate data." });
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

        [HttpGet]
        public IActionResult CHECK_STOCK_PO(string part_code)
        {
            int total_stock = 0;

            string query = @"
        SELECT total_stock FROM tbl_stock_pofinish WHERE part_code = @part_code
    ";

            using (SqlConnection conn = new SqlConnection(DbConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@part_code", part_code);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total_stock = Convert.ToInt32(reader["total_stock"]);
                        }
                    }
                }
            }

            return Json(new { total_stock = total_stock });
        }

        public IActionResult PROCESS_POFINISH(string part_code)
        {
            if (HttpContext.Session.GetString("role") == "PMC")
            {
                int rowsAffected = 0;

                using (SqlConnection conn = new SqlConnection(DbConnection()))
                {
                    conn.Open();

                    // Step 1: Ambil opening_stock dan total_req
                    string checkQuery = @"
                SELECT 
                    m.opening_stock,
                    ISNULL(SUM(a.shot), 0) AS total_req
                FROM [DB_PROMATS].[dbo].[mst_material] m
                LEFT JOIN [DB_PROMATS].[dbo].[mst_accpofinish] a
                    ON m.part_code = a.part_code AND a.status = 'IN-PROGRESS'
                WHERE m.part_code = @part_code
                GROUP BY m.opening_stock";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@part_code", part_code);

                    int openingStock = 0;
                    double totalReq = 0;

                    using (SqlDataReader reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            openingStock = reader.GetInt32(0);
                            totalReq = reader.GetDouble(1);
                        }
                    }

                    // Step 1.5: Jika totalReq = 0, hentikan proses
                    if (totalReq == 0)
                    {
                        return BadRequest("Total request is 0, nothing to process.");
                    }

                    // Step 2: Validasi jika opening_stock cukup
                    if (openingStock >= totalReq)
                    {
                        // Step 3: Lanjutkan UPDATE jika valid
                        string updateQuery = @"
                    UPDATE m
                    SET m.opening_stock = m.opening_stock - ISNULL(t.total_req, 0)
                    FROM [DB_PROMATS].[dbo].[mst_material] AS m
                    LEFT JOIN (
                        SELECT 
                            m.part_code,
                            m.material_id,
                            ISNULL(SUM(a.shot), 0) AS total_req
                        FROM 
                            [DB_PROMATS].[dbo].[mst_material] AS m
                        LEFT JOIN 
                            [DB_PROMATS].[dbo].[mst_accpofinish] AS a
                        ON 
                            m.part_code = a.part_code 
                        AND 
                            a.status = 'IN-PROGRESS'
                        WHERE 
                            m.part_code = @part_code
                        GROUP BY 
                            m.part_code,
                            m.material_id
                    ) AS t ON m.part_code = t.part_code AND m.material_id = t.material_id
                    WHERE m.part_code = @part_code;

                    DECLARE @qty INT;
                    SELECT @qty = SUM(qty) FROM [DB_PROMATS].[dbo].[mst_accpofinish]
                    WHERE part_code = @part_code AND status = 'IN-PROGRESS';

                    UPDATE tbl_stock_pofinish
                    SET total_stock = total_stock + @qty
                    WHERE part_code = @part_code;";

                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@part_code", part_code);

                        rowsAffected = updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        return BadRequest("This request cannot be processed because the material stock is insufficient.");
                    }

                    conn.Close();
                }

                return Json(rowsAffected);
            }
            else
            {
                return RedirectToAction("SignOut", "Login");
            }
        }

        [HttpPost]
        public async Task<JsonResult> UPLOAD_RECEIPT_PMC(IFormFile pmc_order, string request_id, string part_code, string material_id)
        {
            if (HttpContext.Session.GetString("role") != "PMC")
            {
                return Json(new { success = false, message = "You do not have permission." });
            }

            if (pmc_order == null || pmc_order.Length == 0)
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

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PMC_Picture");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var extension = Path.GetExtension(pmc_order.FileName);
                var datetimeNow = DateTime.Now.ToString("yyyyMMddHHmmss");
                var uniqueFileName = $"{part_code}_{material_id}_{datetimeNow}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Jika ukuran file > 1MB, lakukan kompres dan resize dengan rasio 4:3
                if (pmc_order.Length > 1 * 1024 * 1024) // > 1MB
                {

                    using (var image = await SixLabors.ImageSharp.Image.LoadAsync(pmc_order.OpenReadStream()))
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
                        await pmc_order.CopyToAsync(fileStream);
                    }
                }

                // Update DB
                using (SqlConnection con = new SqlConnection(DbConnection()))
                {
                    con.Open();

                    string updateQuery = @"
                UPDATE mst_purchase_request
                SET receipt_proof = @file_order, status = 'CLOSE' 
                WHERE request_id = @request_id
                
                DECLARE @incoming INT;
                SELECT @incoming = quantity FROM [mst_purchase_request] WHERE request_id = @request_id

                UPDATE [mst_material]
                SET opening_stock = opening_stock + @incoming
                WHERE part_code = @part_code AND material_id = @material_id
            ";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@file_order", uniqueFileName);
                        cmd.Parameters.AddWithValue("@request_id", request_id);
                        cmd.Parameters.AddWithValue("@part_code", part_code);
                        cmd.Parameters.AddWithValue("@material_id", material_id);

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

    }
}
