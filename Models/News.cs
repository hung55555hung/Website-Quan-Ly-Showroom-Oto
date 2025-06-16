using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebPBL3.Models;

namespace WebPBL3.Models
{
    public class News
    {
        [Key]
        [StringLength(maximumLength: 10)]
        public string NewsID { get; set; } = string.Empty;


        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Nội dung")]
        
        public string Content { get; set; } = string.Empty ;

        [Display(Name = "Ảnh đính kèm")]

        public string Photo { get; set; } = string.Empty;
        [Display(Name = "Ngày tạo")]
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        
        public string? UpdateBy { get; set; }

        public Staff? Staff { get; set; }
        public string? StaffID { get; set; }
    }
}