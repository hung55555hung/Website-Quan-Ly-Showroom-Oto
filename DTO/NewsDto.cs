using System.ComponentModel.DataAnnotations;
using WebPBL3.Models;

namespace WebPBL3.DTO
{
    public class NewsDTO
    {
        public int? STT;
        public string? NewsID { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Tiêu đề không thể trống")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "Nội dung không thể trống")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Ảnh đính kèm")]
        [Required(ErrorMessage = "Ảnh không thể trống")]
        public string Photo { get; set; } = string.Empty;
        
        [Display(Name = "Ngày tạo")]
        public DateTime CreateAt { get; set; }
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdateAt { get; set; }
        public string? UpdateBy { get; set; }
        [Display(Name = "Tác giả")]
        public string? FullName {  get; set; }
        public string? StaffID { get; set; } = string.Empty;
    }
}
