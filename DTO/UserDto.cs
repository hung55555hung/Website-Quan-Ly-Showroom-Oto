using System.ComponentModel.DataAnnotations;
using WebPBL3.Models;

namespace WebPBL3.DTO
{
	public class UserDTO 
	{	
		

        [Display(Name = "Email")]
        [StringLength(maximumLength: 50)]
        [Required(ErrorMessage = "Bạn phải nhập email")]
        public string Email { get; set; } = string.Empty;

		public string? Password { get; set; }
		public bool Status { get; set; }
		public int RoleID { get; set; }
        public string UserID { get; set; } = string.Empty;

        [Display(Name = "Họ và tên")]
        [StringLength(maximumLength: 50)]
        public string? FullName { get; set; }

        [Display(Name = "Số điện thoại")]
        [StringLength(maximumLength: 15)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Số điện thoại chỉ được chứa các chữ số.")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Số CCCD")]
        [StringLength(maximumLength: 12, MinimumLength = 9, ErrorMessage = "CCCD phải tối thiểu 9 và tối đa 12 chữ số.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "CCCD chỉ được chứa các chữ số.")]
        public string? IdentityCard { get; set; }

        [Display(Name = "Giới tính")]
        public bool? Gender { get; set; }
        [Display(Name = "Ngày sinh")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Địa chỉ")]
        [StringLength(maximumLength: 200, ErrorMessage = "Địa chỉ quá dài.")]
        public string? Address { get; set; }
        [Display(Name = "Ảnh")]
        public string? Photo { get; set; }
		public int? WardID { get; set; }
		public int ProvinceID {  get; set; }
		public int DistrictID {  get; set; }
		public string? WardName { get; set; }
		public string? DistrictName { get; set; }
		public string? ProvinceName { get; set; }

        public string? AccountID { get; set; }


    }
}
