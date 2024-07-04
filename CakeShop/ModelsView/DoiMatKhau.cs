using System.ComponentModel.DataAnnotations;

namespace CakeShop.ModelsView
{
    public class DoiMatKhau
    {

        [Display(Name = "Mật khẩu hiện tại")]
        [Required(ErrorMessage = "Chưa nhập mật khẩu!")]
        [DataType(DataType.Password)]
        public string matKhauHienTai { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Chưa nhập mật khẩu!")]
        [DataType(DataType.Password)]
        public string matKhauMoi { get; set; }

        [Display(Name = "Nhập lại mật khẩu mới")]
        [Required(ErrorMessage = "Chưa nhập mật khẩu!")]
        [DataType(DataType.Password)]
        public string matKhauMoiNhapLai { get; set; }
    }
}
