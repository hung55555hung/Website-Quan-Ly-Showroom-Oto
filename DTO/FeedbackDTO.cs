using System.ComponentModel.DataAnnotations;

namespace WebPBL3.DTO
{
    public class FeedbackDTO
    {
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Bạn chưa nhập họ và tên")]
        public string FullName { get; set; } = string.Empty;
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Bạn chưa nhập địa chỉ email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Bạn chưa nhập tiêu đề")]
        public string Title { get; set; } = string.Empty;


        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "Bạn chưa nhập nội dung")]
        public string Content { get; set; } = string.Empty;
        [Display(Name = "Đánh giá")]
        [Required(ErrorMessage = "Mức độ hài lòng không thể trống")]
        public int Rating { get; set; }
    }
}
