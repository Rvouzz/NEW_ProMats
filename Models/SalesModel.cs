namespace ProMats.Models
{
    public class POFinishModel
    {
        public string? po_id { get; set; }
        public string? part_code { get; set; }
        public string? part_name { get; set; }
        public string? qty { get; set; }
        public string? date { get; set; }
        public string? status { get; set; }
        public string? request_by { get; set; }
        public string? file_order { get; set; }
        public string? receipt_proof { get; set; }
    }

    public class DetailSalesModel
    {
        public string? po_id { get; set; }
        public string? part_code { get; set; }
        public string? part_name { get; set; }
        public string? deliver_by { get; set; }
        public string? finish_date { get; set; }
        public string? remark { get; set; }
        public string? receipt_proof { get; set; }
    }
}
