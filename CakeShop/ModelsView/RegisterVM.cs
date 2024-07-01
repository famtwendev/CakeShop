using System.ComponentModel.DataAnnotations;

namespace CakeShop.ModelsView
{
    public class RegisterVM
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự!")]
        public string MaKh { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự!")]
        public string HoTen { get; set; }

        [Display(Name = "Giới tính")]
        public bool GioiTinh { get; set; } = true;

        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Display(Name = "Địa chỉ")]
        [MaxLength(60, ErrorMessage = "Tối đa 60 ký tự!")]
        public string DiaChi { get; set; }

        [Display(Name = "Điện thoại")]
        [MaxLength(24, ErrorMessage = "Tối đa 24 ký tự!")]
        [RegularExpression(@"0[9875]\d{8}", ErrorMessage = "Chưa đúng định dạng SĐT!")]
        public string? DienThoai { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email!")]
        [Required(ErrorMessage = "*")]
        public string Email { get; set; }

        [Display(Name = "Hình ảnh đại diện")]
        public string? Hinh { get; set; }
    }
}
