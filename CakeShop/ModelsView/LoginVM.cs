using System.ComponentModel.DataAnnotations;

namespace CakeShop.ModelsView
{
    public class LoginVM
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Chưa nhập tên đăng nhập!")]
        [MaxLength(30, ErrorMessage = "Tối đa 30 ký tự!")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Chưa nhập mật khẩu!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
