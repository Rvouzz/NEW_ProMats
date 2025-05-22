using ProMats.Models;
using System.Data;
using System.Data.SqlClient;

namespace ProMats.Function
{
    public class DatabaseAccessLayer
    {
        public string ConnectionString = "Data Source=.\\MSSQLSERVER01;Initial Catalog=DB_PROMATS;Persist Security Info=True;User ID=sa;Password=0602@Feb;MultipleActiveResultSets=true";


        // <----------------------------------------> FOR PMC <----------------------------------------> //
        // === PO FINISH CONFIGURATION === //
        public async Task<List<POFinishModel>> GET_POFINISHPMC(string date_from, string date_to)
        {
            List<POFinishModel> materials = new List<POFinishModel>();

            // Query SQL dengan parameter filter
            string query = @"
    SELECT po_id, part_code, qty, date, status, request_by, part_name, file_order
    FROM mst_pofinish
    WHERE 
        (@date_from IS NULL OR date >= @date_from) AND
        (@date_to IS NULL OR date <= @date_to) AND 
        status = 'OPEN' ORDER BY po_id DESC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Menambahkan parameter ke SqlCommand
                    cmd.Parameters.AddWithValue("@date_from", string.IsNullOrEmpty(date_from) ? (object)DBNull.Value : date_from);
                    cmd.Parameters.AddWithValue("@date_to", string.IsNullOrEmpty(date_to) ? (object)DBNull.Value : date_to);

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            POFinishModel mat = new POFinishModel
                            {
                                po_id = reader["po_id"].ToString(),
                                part_code = reader["part_code"].ToString(),
                                part_name = reader["part_name"].ToString(),
                                qty = reader["qty"].ToString(),
                                date = reader["date"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["date"]).ToString("MM/dd/yyyy")
                                    : string.Empty,
                                request_by = reader["request_by"].ToString(),
                                status = reader["status"].ToString(),
                                file_order = reader["file_order"].ToString()
                            };

                            materials.Add(mat);
                        }
                    }
                }
            }

            return materials;
        }


        public async Task<List<POAccModel>> GET_POFINISH_ACC(string date_from, string date_to)
        {
            List<POAccModel> materials = new List<POAccModel>();
            // Query SQL dengan parameter filter
            string query = @"
            SELECT *
            FROM mst_accpofinish
            WHERE 
                (@date_from IS NULL OR date >= @date_from) AND
                (@date_to IS NULL OR date <= @date_to) AND status = 'IN-PROGRESS'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Menambahkan parameter ke SqlCommand
                    cmd.Parameters.AddWithValue("@date_from", string.IsNullOrEmpty(date_from) ? (object)DBNull.Value : date_from);
                    cmd.Parameters.AddWithValue("@date_to", string.IsNullOrEmpty(date_to) ? (object)DBNull.Value : date_to);

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            POAccModel mat = new POAccModel
                            {
                                id_acc = reader["id_acc"].ToString(),
                                po_id = reader["po_id"].ToString(),
                                part_code = reader["part_code"].ToString(),
                                part_name = reader["part_name"].ToString(),
                                qty = reader["qty"].ToString(),
                                shot = Math.Round(Convert.ToDecimal(reader["shot"]), 2).ToString(),
                                part = reader["part"].ToString(),
                                runner = reader["runner"].ToString(),
                                date = reader["date"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["date"]).ToString("MM/dd/yyyy")
                                    : string.Empty,
                                //date = DateTime.Parse(reader["date"].ToString()).ToString("dd MMM yyyy"),
                                request_by = reader["request_by"].ToString(),
                                status = reader["status"].ToString(),
                                file_order = reader["file_order"].ToString(),
                            };
                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }



        // === DASHBOARD CONFIGURATION === //
        public async Task<List<DashboardModel>> GET_DASHBOARD_PART()
        {
            List<DashboardModel> materials = new List<DashboardModel>();
            string query = @"
        SELECT [material_id], 
               [material_name], 
               [opening_stock], 
               [incoming], 
               [total_req]
        FROM [DB_PROMATS].[dbo].[mst_part]
        WHERE (opening_stock + incoming) < total_req";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text; // Changed to Text since the query is a direct SQL query
                    cmd.Connection = conn;
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DashboardModel mat = new DashboardModel
                            {
                                material_id = reader["material_id"].ToString(),
                                material_name = reader["material_name"].ToString(),
                                opening_stock = Convert.ToInt32(reader["opening_stock"]),
                                incoming = Convert.ToInt32(reader["incoming"]),
                                total_req = Convert.ToInt32(reader["total_req"])
                            };

                            // Perhitungan shortage_qty
                            mat.total_stock = mat.opening_stock + mat.incoming;
                            mat.shortage_qty = mat.total_stock - mat.total_req;

                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }




        // === PART DATA CONFIGURATION === //
        public async Task<List<MstPartModel>> GET_MASTER_PART()
        {
            List<MstPartModel> materials = new List<MstPartModel>();
            string query = "SELECT * FROM mst_part ORDER BY part_code ASC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync(); // Buka koneksi secara asinkron

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync()) // Baca data secara asinkron
                        {
                            MstPartModel mat = new MstPartModel
                            {
                                id_part = reader["id_part"]?.ToString(),
                                part_code = reader["part_code"]?.ToString(),
                                part_name = reader["part_name"]?.ToString(),
                                part = reader["part"]?.ToString(),
                                runner = reader["runner"]?.ToString(),
                            };
                            materials.Add(mat); // Tambahkan ke daftar hasil
                        }
                    }
                }
            }
            return materials; // Kembalikan daftar bahan
        }

        public async Task<List<PartModel>> GET_MASTER_MATERIAL(string pcode)
        {
            List<PartModel> materials = new List<PartModel>();
            string query = @"
                            SELECT 
                                m.id_mtl,
                                m.material_id,
                                m.material_name,
                                m.opening_stock,
                                m.incoming,
                                ISNULL(SUM(a.shot), 0) AS total_req,
                                m.record_date,
                                m.part_code
                            FROM 
                                [DB_PROMATS].[dbo].[mst_material] AS m
                            LEFT JOIN 
                                [DB_PROMATS].[dbo].[mst_accpofinish] AS a
                            ON 
                                m.part_code = a.part_code 
                            AND 
                                a.status = 'IN-PROGRESS'
                            WHERE 
                                (@pcode IS NULL OR m.part_code = @pcode)
                            GROUP BY 
                                m.id_mtl,
                                m.material_id,
                                m.material_name,
                                m.opening_stock,
                                m.incoming,
                                m.record_date,
                                m.part_code
                            ORDER BY 
                                m.material_id;
                            ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pcode", (object)pcode ?? DBNull.Value);
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PartModel mat = new PartModel
                            {
                                id_mtl = reader["id_mtl"]?.ToString(),
                                part_code = reader["part_code"]?.ToString(),
                                material_id = reader["material_id"]?.ToString(),
                                material_name = reader["material_name"]?.ToString(),
                                opening_stock = reader["opening_stock"] != DBNull.Value
                                    ? Convert.ToInt32(reader["opening_stock"])
                                    : 0,
                                incoming = reader["incoming"] != DBNull.Value
                                    ? Convert.ToInt32(reader["incoming"])
                                    : 0,
                                total_req = reader["total_req"] != DBNull.Value
                                    ? Convert.ToSingle(reader["total_req"])
                                    : 0f,
                            };

                            mat.shortage_qty = mat.opening_stock.HasValue && mat.incoming.HasValue && mat.total_req.HasValue
                                ? (float?)Math.Round((mat.opening_stock.Value + mat.incoming.Value - mat.total_req.Value), 2)
                                : null;

                            materials.Add(mat);
                        }
                    }
                }
            }

            return materials;
        }











        // === PURCHASE REQUEST CONFIGURATION === //
        public async Task<List<PRModel>> GET_PURCHASE_REQUEST(string date_from, string date_to, string status)
        {
            List<PRModel> materials = new List<PRModel>();
            // Query SQL dengan parameter filter
            string query = @"
            SELECT request_id, material_id, part_code, quantity, date, request_by, status
            FROM mst_purchase_request
            WHERE 
                (@date_from IS NULL OR date >= @date_from) AND
                (@date_to IS NULL OR date <= @date_to) AND
                (@status IS NULL OR status = @status)
            ORDER BY 
                CASE UPPER(TRIM(status))
                    WHEN 'SHIPPING' THEN 1
                    WHEN 'OPEN' THEN 2
                    WHEN 'IN-PROGRESS' THEN 3
                    WHEN 'CLOSE' THEN 4
                    WHEN 'REJECT' THEN 5
                    ELSE 6
                END
            ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Menambahkan parameter ke SqlCommand
                    cmd.Parameters.AddWithValue("@date_from", string.IsNullOrEmpty(date_from) ? (object)DBNull.Value : date_from);
                    cmd.Parameters.AddWithValue("@date_to", string.IsNullOrEmpty(date_to) ? (object)DBNull.Value : date_to);
                    cmd.Parameters.AddWithValue("@status", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status);

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PRModel mat = new PRModel
                            {
                                request_id = reader["request_id"].ToString(),
                                part_code = reader["part_code"].ToString(),
                                material_id = reader["material_id"].ToString(),
                                quantity = reader["quantity"].ToString(),
                                date = reader["date"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["date"]).ToString("MM/dd/yyyy")
                                    : string.Empty,
                                request_by = reader["request_by"].ToString(),
                                status = reader["status"].ToString(),
                            };
                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }

        // ~~~~~ FG TRACKING CONFIGURATION ~~~~~//
        public async Task<List<FGTrackingModel>> GET_FG_TRACKING()
        {
            List<FGTrackingModel> materials = new List<FGTrackingModel>();
            string query = @"
            SELECT 
                b.id_det, 
                a.part_code, 
                a.part_name, 
                b.total_stock,
                COALESCE(SUM(c.qty), 0) AS total_request,
                CASE 
                    WHEN b.total_stock >= COALESCE(SUM(c.qty), 0) THEN '<span class=""text-success"">Available</span>'
                    ELSE '<span class=""text-danger blink"">Restock Needed!!</span>'
                END AS status,
                b.update_date
            FROM mst_part a 
            LEFT JOIN tbl_stock_pofinish b ON a.id_part = b.id_part
            LEFT JOIN mst_accpofinish c ON a.part_code = c.part_code AND c.status = 'IN-PROGRESS'
            GROUP BY 
                b.id_det, 
                a.part_code, 
                a.part_name, 
                b.total_stock, 
                b.update_date
            ";


            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            FGTrackingModel mat = new FGTrackingModel
                            {
                                id_det = reader["id_det"].ToString(),
                                part_code = reader["part_code"].ToString(),
                                part_name = reader["part_name"].ToString(),
                                total_stock = reader["total_stock"].ToString(),
                                total_request = reader["total_request"].ToString(),
                                update_date = reader["update_date"].ToString(),
                                status = reader["status"].ToString(),
                            };
                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }

        // === HISTORY CONFIGURATION === //
        public async Task<List<POFinishModel>> GET_HISTORY_PMC(string date_from, string date_to, string status)
        {
            List<POFinishModel> materials = new List<POFinishModel>();
            // Query SQL dengan parameter filter
            string query = @"
            SELECT po_id, part_code, qty, date, status, request_by, part_name, file_order
            FROM mst_pofinish
            WHERE 
                (@date_from IS NULL OR date >= @date_from) AND
                (@date_to IS NULL OR date <= @date_to) AND
                (
                    @status IS NULL AND status IN ('CLOSE', 'REJECTED', 'SHIPPING') OR
                    @status IS NOT NULL AND status = @status
                )
            ORDER BY 
                CASE UPPER(TRIM(status))
                    WHEN 'SHIPPING' THEN 1
                    WHEN 'IN-PROGRESS' THEN 2
                    WHEN 'OPEN' THEN 3
                    WHEN 'CLOSE' THEN 4
                    WHEN 'REJECT' THEN 5
                    ELSE 6
                END";


            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Menambahkan parameter ke SqlCommand
                    cmd.Parameters.AddWithValue("@date_from", string.IsNullOrEmpty(date_from) ? (object)DBNull.Value : date_from);
                    cmd.Parameters.AddWithValue("@date_to", string.IsNullOrEmpty(date_to) ? (object)DBNull.Value : date_to);
                    cmd.Parameters.AddWithValue("@status", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status);

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            POFinishModel mat = new POFinishModel
                            {
                                po_id = reader["po_id"].ToString(),
                                part_code = reader["part_code"].ToString(),
                                part_name = reader["part_name"].ToString(),
                                qty = reader["qty"].ToString(),
                                date = reader["date"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["date"]).ToString("MM/dd/yyyy")
                                    : string.Empty,
                                request_by = reader["request_by"].ToString(),
                                status = reader["status"].ToString(),
                                file_order = reader["file_order"].ToString(),
                            };
                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }










        // <----------------------------------------> FOR SALES <----------------------------------------> //
        // === PO FINISH CONFIGURATION === //
        public async Task<List<POFinishModel>> GET_POFINISH_GOOD(string date_from, string date_to, string status)
        {
            List<POFinishModel> materials = new List<POFinishModel>();
            // Query SQL dengan parameter filter
            string query = @"
            SELECT po_id, part_code, qty, date, status, request_by, part_name, file_order
            FROM mst_pofinish
            WHERE 
                (@date_from IS NULL OR date >= @date_from) AND
                (@date_to IS NULL OR date <= @date_to) AND
                (@status IS NULL OR status = @status)
            ORDER BY 
                CASE UPPER(TRIM(status))
                    WHEN 'SHIPPING' THEN 1
                    WHEN 'IN-PROGRESS' THEN 2
                    WHEN 'OPEN' THEN 3
                    WHEN 'CLOSE' THEN 4
                    WHEN 'REJECT' THEN 5
                    ELSE 6
                END";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Menambahkan parameter ke SqlCommand
                    cmd.Parameters.AddWithValue("@date_from", string.IsNullOrEmpty(date_from) ? (object)DBNull.Value : date_from);
                    cmd.Parameters.AddWithValue("@date_to", string.IsNullOrEmpty(date_to) ? (object)DBNull.Value : date_to);
                    cmd.Parameters.AddWithValue("@status", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status);

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            POFinishModel mat = new POFinishModel
                            {
                                po_id = reader["po_id"].ToString(),
                                part_code = reader["part_code"].ToString(),
                                part_name = reader["part_name"].ToString(),
                                qty = reader["qty"].ToString(),
                                date = reader["date"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["date"]).ToString("MM/dd/yyyy")
                                    : string.Empty,
                                request_by = reader["request_by"].ToString(),
                                status = reader["status"].ToString(),
                                file_order = reader["file_order"].ToString(),
                            };
                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }

        public List<DetailSalesModel> GET_DETAIL_SALES(string po_id)
        {
            List<DetailSalesModel> materials = new List<DetailSalesModel>();
            string query = @"
        SELECT d.po_id, d.deliver_by, d.finish_date, d.remark, 
               m.part_code, m.part_name, m.receipt_proof
        FROM [DB_PROMATS].[dbo].[tbl_detail_finish] d
        JOIN [DB_PROMATS].[dbo].[mst_pofinish] m 
            ON d.po_id = m.po_id
        WHERE d.po_id = @po_id";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@po_id", po_id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DetailSalesModel mat = new DetailSalesModel();
                            mat.po_id = reader["po_id"].ToString();
                            mat.part_code = reader["part_code"].ToString();
                            mat.part_name = reader["part_name"].ToString();
                            mat.deliver_by = reader["deliver_by"].ToString();
                            mat.finish_date = ((DateTime)reader["finish_date"]).ToString("dd MMM yyyy hh:mm tt");
                            mat.remark = reader["remark"].ToString();
                            mat.receipt_proof = reader["receipt_proof"].ToString();
                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }





        // <----------------------------------------> FOR PURCHASING <----------------------------------------> //
        public async Task<List<PRModel>> GET_PURCHASING_REQ(string date_from, string date_to)
        {
            List<PRModel> materials = new List<PRModel>();
            // Query SQL dengan parameter filter
            string query = @"
            SELECT request_id, material_id, part_code, quantity, date, request_by, status
            FROM mst_purchase_request
            WHERE 
                (@date_from IS NULL OR date >= @date_from) AND
                (@date_to IS NULL OR date <= @date_to) AND
                status = 'OPEN'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Menambahkan parameter ke SqlCommand
                    cmd.Parameters.AddWithValue("@date_from", string.IsNullOrEmpty(date_from) ? (object)DBNull.Value : date_from);
                    cmd.Parameters.AddWithValue("@date_to", string.IsNullOrEmpty(date_to) ? (object)DBNull.Value : date_to);

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PRModel mat = new PRModel
                            {
                                request_id = reader["request_id"].ToString(),
                                part_code = reader["part_code"].ToString(),
                                material_id = reader["material_id"].ToString(),
                                quantity = reader["quantity"].ToString(),
                                date = reader["date"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["date"]).ToString("MM/dd/yyyy")
                                    : string.Empty,
                                request_by = reader["request_by"].ToString(),
                                status = reader["status"].ToString(),
                            };
                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }

        public async Task<List<PRModel>> GET_PROSES_REQ(string date_from, string date_to)
        {
            List<PRModel> materials = new List<PRModel>();
            // Query SQL dengan parameter filter
            string query = @"
            SELECT request_id, material_id, part_code, quantity, date, request_by, status
            FROM mst_purchase_request
            WHERE 
                (@date_from IS NULL OR date >= @date_from) AND
                (@date_to IS NULL OR date <= @date_to) AND
                status = 'IN-PROGRESS'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Menambahkan parameter ke SqlCommand
                    cmd.Parameters.AddWithValue("@date_from", string.IsNullOrEmpty(date_from) ? (object)DBNull.Value : date_from);
                    cmd.Parameters.AddWithValue("@date_to", string.IsNullOrEmpty(date_to) ? (object)DBNull.Value : date_to);

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PRModel mat = new PRModel
                            {
                                request_id = reader["request_id"].ToString(),
                                part_code = reader["part_code"].ToString(),
                                material_id = reader["material_id"].ToString(),
                                quantity = reader["quantity"].ToString(),
                                date = reader["date"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["date"]).ToString("MM/dd/yyyy")
                                    : string.Empty,
                                request_by = reader["request_by"].ToString(),
                                status = reader["status"].ToString(),
                            };
                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }

        public async Task<List<PRModel>> GET_HISTORY_REQ(string date_from, string date_to, string status)
        {
            List<PRModel> materials = new List<PRModel>();
            // Query SQL dengan parameter filter
            string query = @"
            SELECT request_id, material_id, part_code, quantity, date, request_by, status
            FROM mst_purchase_request
            WHERE 
                (@date_from IS NULL OR date >= @date_from) AND
                (@date_to IS NULL OR date <= @date_to) AND
                (
                    @status IS NULL AND status IN ('SHIPPING','CLOSE', 'REJECTED') OR
                    @status IS NOT NULL AND status = @status
                )
            ORDER BY 
                CASE UPPER(TRIM(status))
                    WHEN 'SHIPPING' THEN 1
                    WHEN 'CLOSE' THEN 2
                    WHEN 'REJECTED' THEN 3
                    ELSE 4
                END";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Menambahkan parameter ke SqlCommand
                    cmd.Parameters.AddWithValue("@date_from", string.IsNullOrEmpty(date_from) ? (object)DBNull.Value : date_from);
                    cmd.Parameters.AddWithValue("@date_to", string.IsNullOrEmpty(date_to) ? (object)DBNull.Value : date_to);
                    cmd.Parameters.AddWithValue("@status", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status);

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PRModel mat = new PRModel
                            {
                                request_id = reader["request_id"].ToString(),
                                part_code = reader["part_code"].ToString(),
                                material_id = reader["material_id"].ToString(),
                                quantity = reader["quantity"].ToString(),
                                date = reader["date"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["date"]).ToString("MM/dd/yyyy")
                                    : string.Empty,
                                request_by = reader["request_by"].ToString(),
                                status = reader["status"].ToString(),
                            };
                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }

        public List<PRModel> GET_DETAIL_PURCHASING(string request_id)
        {
            List<PRModel> materials = new List<PRModel>();
            string query = @"
        SELECT d.request_id, d.supplier_name, d.lead_time, d.shipment, d.total_time, d.acc_by,
               m.part_code, m.material_id, m.quantity, m.receipt_proof
        FROM [DB_PROMATS].[dbo].[tbl_detail_pr] d
        JOIN [DB_PROMATS].[dbo].[mst_purchase_request] m 
            ON d.request_id = m.request_id
        WHERE d.request_id = @request_id";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@request_id", request_id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PRModel mat = new PRModel();
                            mat.request_id = reader["request_id"].ToString();
                            mat.supplier_name = reader["supplier_name"].ToString();
                            mat.lead_time = reader["lead_time"].ToString();
                            mat.shipment = reader["shipment"].ToString();
                            mat.total_time = reader["total_time"].ToString();
                            mat.acc_by = reader["acc_by"].ToString();
                            mat.part_code = reader["part_code"].ToString();
                            mat.material_id = reader["material_id"].ToString();
                            mat.quantity = reader["quantity"].ToString();
                            mat.receipt_proof = reader["receipt_proof"].ToString();
                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }


        // ----- ADMIN PANEL ----- //

        public async Task<List<LoginModel>> GET_USER_DATA()
        {
            List<LoginModel> materials = new List<LoginModel>();
            string query = @"SELECT * FROM mst_users ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            LoginModel mat = new LoginModel
                            {
                                user_id = reader["user_id"].ToString(),
                                username = reader["username"].ToString(),
                                name = reader["name"].ToString(),
                                password = reader["password"].ToString(),
                                role = reader["role"].ToString(),
                                login_id = reader["login_id"].ToString(),
                                last_update = reader["last_update"].ToString(),
                            };
                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }


    }
}
