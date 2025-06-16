using WebPBL3.Models;
namespace WebPBL3.DTO
{
    public class FullStaffInfoDTO
    {
        public Staff Staff { get; set; }
        public User User { get; set; }
        public Account Account { get; set; }
        public string? WardName { get; set; }
        public string? DistrictName { get; set; }
        public string? ProvinceName { get; set; }
        public int? WardID { get; set; }
        public int? DistrictID { get; set; }
        public int? ProvinceID { get; set; }
    }
}
