namespace ProMats.Models
{
    public class KeyModel
    {
        public string? Text { get; set; }
        public string? Id { get; set; }

    }

    public class FGTrackingModel
    {
        public string? id_det { get; set; }
        public string? part_code { get; set; }
        public string? part_name { get; set; }
        public string? total_stock { get; set; }
        public string? total_request { get; set; }
        public string? update_date { get; set; }
        public string? status { get; set; }
    }
    public class PRModel
    {
        public string? part_code { get; set; }
        public string? request_id { get; set; }
        public string? material_id { get; set; }
        public string? quantity { get; set; }
        public string? date { get; set; }
        public string? request_by { get; set; }
        public string? status { get; set; }
        public string? remark { get; set; }
        public string? lead_time { get; set; }
        public string? supplier_name { get; set; }
        public string? shipment { get; set; }
        public string? total_time { get; set; }
        public string? acc_by { get; set; }
        public string? receipt_proof { get; set; }
    }

    public class POAccModel
    {
        public string? id_acc { get; set; }
        public string? po_id { get; set; }
        public string? part_code { get; set; }
        public string? part_name { get; set; }
        public string? qty { get; set; }
        public string? shot { get; set; }
        public string? part { get; set; }
        public string? runner { get; set; }
        public string? date { get; set; }
        public string? request_by { get; set; }
        public string? status { get; set; }
        public string? file_order { get; set; }
    }
    public class PartModel
    {
        public string? id_mtl { get; set; }
        public string? material_id { get; set; }
        public string? material_name { get; set; }
        public int? opening_stock { get; set; }
        public int? incoming { get; set; }
        public float? total_req { get; set; }
        public float? shortage_qty { get; set; }
        public string? part_code { get; set; }
    }

    public class MstPartModel
    {
        public string? id_part { get; set; }
        public string? part_code { get; set; }
        public string? part_name { get; set; }
        public string? shot { get; set; }
        public string? part { get; set; }
        public string? runner { get; set; }
    }

    public class DashboardModel
    {
        public string? material_id { get; set; }
        public string? material_name { get; set; }
        public int? total_stock { get; set; }
        public int? opening_stock { get; set; }
        public int? incoming { get; set; }
        public int? total_req { get; set; }
        public int? shortage_qty { get; set; }
        public int? total_shortage { get; set; }
        public int? total { get; set; }
        public int? approved { get; set; }
        public int? waiting { get; set; }
        public int? rejected { get; set; }
        public int? in_progress { get; set; }
        public int? month { get; set; }
        public int? count { get; set; }
        public int? open { get; set; }
        public int? close { get; set; }
    }

}
