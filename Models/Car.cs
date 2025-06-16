using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WebPBL3.Models
{
    public class Car
    {
        public Car()
        {
            CarID = CarName = Origin = Dimension = Photo = Color = Engine = Description = FuelConsumption = string.Empty;
            Price = Capacity = 0.0;
            Topspeed = 0.0F;
            Flag = false;
            Seat = Year = Quantity = 0;
        }
        [Key]
        [Display(Name = "Mã xe")]
        [StringLength(maximumLength: 10)]
        public string CarID { get; set; }


        
        [Display(Name = "Tên xe")]
        [StringLength(maximumLength: 50)]
        public string CarName { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Giá xe")]
        public double Price { get; set; }
        [Display(Name = "Số chỗ")]

        public int Seat { get; set; }

        [Display(Name = "Xuất xứ")]
        [StringLength(maximumLength: 50)]
        public string Origin { get; set; }

        [StringLength(maximumLength: 50)]
        [Display(Name = "Kích thước")]

        public string Dimension { get; set; }
       

        [Display(Name = "Dung tích")]
        
        public double Capacity { get; set; }

        [Display(Name = "Tốc độ tối đa")]
        [Range(0, float.MaxValue)]
        public float Topspeed { get; set; }

        [Display(Name = "Màu sắc")]
        [StringLength(maximumLength: 50)]
        
        public string Color { get; set; }

        [Display(Name = "Ảnh")]
        
        public string Photo { get; set; }
        [Display(Name = "Năm sản xuất")]
        [Range(1000, int.MaxValue)]
        public int Year { get; set; }
        [Display(Name = "Động cơ")]
        [StringLength(maximumLength: 50)]
        
        public string Engine { get; set; }
        [Display(Name = "Số lượng")]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Mức tiêu thụ")]
        [StringLength(maximumLength: 50)]
        public string FuelConsumption { get; set; }
        public bool Flag { get; set; }
        
        public Make? Make { get; set; }
        [Display(Name = "Mã hãng")]
        public int MakeID { get; set; }
    }
}