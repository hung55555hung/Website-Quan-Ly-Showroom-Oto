using System.ComponentModel.DataAnnotations;

namespace WebPBL3.DTO
{
    public class RegisterDTO
    {
        [Display(Name = "Email")]
        [StringLength(maximumLength: 50)]
        [Required(ErrorMessage = "Bạn chưa nhập địa chỉ email")]
        public string Email { get; set; }  =string.Empty;

        [Display(Name = "Mật khẩu")]
        [StringLength(maximumLength: 50)]
        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Nhập lại mật khẩu")]
        [StringLength(maximumLength: 50)]
        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu nhập lại")]
        public string RetypePassword { get; set; } = string.Empty;

    }
}
