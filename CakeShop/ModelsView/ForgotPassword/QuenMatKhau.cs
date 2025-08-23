using System.ComponentModel.DataAnnotations;

namespace CakeShop.ModelsView.ForgotPassword
{
    public class QuenMatKhau
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email!")]
        [Required(ErrorMessage ="Chưa nhập email")]
        public string Email { get; set; } = null!;
	}
}
