namespace WebPBL3.DTO.Statistic
{
    public class StatisticTable
    {
        public int STT { get; set; }
        public string CarID { get; set; } = string.Empty;
        public string MakeName { get; set; } = string.Empty;
        public string StaffID { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
        
        
       
    }
}
