using System.ComponentModel.DataAnnotations;

namespace WebPBL3.DTO
{
    public class CarDTO
    {
        public string? CarID { get; set; }

        [Display(Name = "Tên xe")]
        [Required(ErrorMessage = "Tên xe không thể trống")]
        [StringLength(maximumLength: 50)]
        public string CarName { get; set; } = string.Empty;

        [Display(Name = "Giá xe")]
        [Required(ErrorMessage = "Giá xe không thể trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá xe không thể là số âm.")]
        
        public double Price { get; set; }

        [Display(Name = "Số chỗ")]
        [Required(ErrorMessage = "Số chỗ không thể trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số chỗ không thể là số âm.")]
        public int Seat { get; set; }

        [Display(Name = "Xuất xứ")]
        [StringLength(maximumLength: 50)]
        [Required(ErrorMessage = "Xuất xứ không thể trống")]
        public string Origin { get; set; } = string.Empty;

        [StringLength(maximumLength: 50)]
        [Display(Name = "Kích thước")]
        [Required(ErrorMessage = "Kích thước không thể trống")]
        public string Dimension { get; set; } = string.Empty;

        [Display(Name = "Dung tích")]
        [Required(ErrorMessage = "Dung tích không thể trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Dung tích không thể là số âm.")]
        public double Capacity { get; set; }
        [Display(Name = "Tốc độ tối đa")]
        [Required(ErrorMessage = "Tốc độ không thể trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Tốc độ không thể là số âm.")]
        public float Topspeed { get; set; }


        [Display(Name = "Màu sắc")]
        [StringLength(maximumLength: 50)]
        [Required(ErrorMessage = "Màu sắc không thể trống")]
        public string Color { get; set; } = string.Empty;

        [Display(Name = "Ảnh")]
        [Required(ErrorMessage = "Ảnh không thể trống")]
        public string Photo { get; set; } = string.Empty;
        [Display(Name = "Năm sản xuất")]
        [Required(ErrorMessage = "Năm không thể trống")]
        [Range(1000, int.MaxValue, ErrorMessage = "Năm sản xuất không nhỏ hơn 1000")]
        public int Year { get; set; }
        [Display(Name = "Động cơ")]
        [StringLength(maximumLength: 50)]
        [Required(ErrorMessage = "Động cơ không thể trống")]
        public string Engine { get; set; } = string.Empty;
        [Display(Name = "Số lượng")]
        [Required(ErrorMessage = "Số lượng không thể trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng xe không nhỏ hơn 1")]
        public int Quantity { get; set; }

        [Display(Name = "Mô tả")]
        [Required(ErrorMessage = "Mô tả không thể trống")]
        public string Description { get; set; } = string.Empty;
        [Display(Name = "Mức tiêu thụ")]
        [StringLength(maximumLength: 50)]
        [Required(ErrorMessage = "Mức tiêu thụ không thể trống")]
        public string FuelConsumption { get; set; } = string.Empty;
        
        public string? MakeName { get; set; }
        
        public int MakeID { get; set; }
        
    }
}
