using CakeShop.Data;
using System.ComponentModel.DataAnnotations;

namespace CakeShop.ModelsView.Admin
{
    public class AdminLoginVM
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Chưa nhập tên đăng nhập!")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự!")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Chưa nhập mật khẩu!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
