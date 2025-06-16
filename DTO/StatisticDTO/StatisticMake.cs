using System.ComponentModel.DataAnnotations;

namespace WebPBL3.DTO.Statistic
{
    public class StatisticMake
    {

        public string MakeName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Revenue { get; set; }
        
    }
}
