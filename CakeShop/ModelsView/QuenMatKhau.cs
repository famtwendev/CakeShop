using System.ComponentModel.DataAnnotations;

namespace CakeShop.ModelsView
{
    public class QuenMatKhau
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Chưa nhập tên đăng nhập!")]
        [MaxLength(30, ErrorMessage = "Tối đa 30 ký tự!")]
        public string Email { get; set; }

        public string OTP { get; set; }
    }
}
