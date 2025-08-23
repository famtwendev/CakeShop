using CakeShop.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CakeShop.Areas.Admin.Models
{
    public class ChiTietHangHoa
    {
        // Table: HangHoa
        [DisplayName("Mã hàng hóa")]
        [Required(ErrorMessage = "*")]
        public int MaHh { get; set; }

        [DisplayName("Tên hàng hóa")]
        [Required(ErrorMessage = "*")]
        public string TenHh { get; set; } = null!;

        [DisplayName("Tên loại Alias")]
        public string? TenAlias { get; set; }
        
        [DisplayName("Hình ảnh chính")]
        public string? Hinh { get; set; }

        [Required(ErrorMessage = "*")]
        public int MaLoai { get; set; }

        [DisplayName("Kích thước")]
        [Required(ErrorMessage = "*")]
        public string? MoTaDonVi { get; set; }

        [DisplayName("Đơn giá")]
        [Required(ErrorMessage = "*")]
        public double DonGia { get; set; }

        [DisplayName("Ngày sản xuất")]
        public DateTime NgaySx { get; set; }

        [DisplayName("Giảm giá")]
        [Required(ErrorMessage = "*")]
        public double GiamGia { get; set; }

        [DisplayName("Số lượt xem")]
        public int SoLanXem { get; set; }

        [DisplayName("Mô tả")]
        public string? MoTa { get; set; }
        [DisplayName("Mã nhà cung")]
        public string MaNcc { get; set; } = null!;
        public ICollection<HinhanhSp> HinhanhSps { get; set; } = new List<HinhanhSp>();

    }
}
