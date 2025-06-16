using System.Diagnostics;

namespace WebPBL3.DTO
{
    public class DetailOrderDTO
    {
        public string OrderId {  get; set; } = string.Empty;
        public string CustomerName {  get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string EmailCustomer {  get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string StaffId { get; set; } = string.Empty;
        public string StaffName {  get; set; } = string.Empty;
        public string EmailStaff {  get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public double ToTalPrice {  get; set; }
        public string Status {  get; set; } = string.Empty;
        public List<Items> items { get; set; }=new List<Items> { };
    }
}
