using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CakeShop.Data;

public partial class HangHoa
{
    [DisplayName("Mã hàng hóa")]
    public int MaHh { get; set; }

    [DisplayName("Tên hàng hóa")]
    public string TenHh { get; set; } = null!;

    [DisplayName("Tên loại Alias")]
    public string? TenAlias { get; set; }

    public int MaLoai { get; set; }

    [DisplayName("Kích thước")]
    public string? MoTaDonVi { get; set; }

    [DisplayName("Đơn giá")]
    public double DonGia { get; set; }

    [DisplayName("Hình ảnh")]
    public string Hinh { get; set; } = null!;

    [DisplayName("Ngày sản xuất")]
    public DateTime NgaySx { get; set; }

    [DisplayName("Giảm giá")]
    public double GiamGia { get; set; }

    [DisplayName("Số lượt xem")]
    public int SoLanXem { get; set; }
    
    [DisplayName("Mô tả")]
    public string? MoTa { get; set; }
    public string MaNcc { get; set; } = null!;

    public virtual ICollection<BanBe>? BanBes { get; set; } = new List<BanBe>();

    public virtual ICollection<ChiTietHd>? ChiTietHds { get; set; } = new List<ChiTietHd>();

    public virtual ICollection<HinhanhSp>? HinhanhSps { get; set; } = new List<HinhanhSp>();

    public string MaHhTenHh => $"{MaHh} ~ {TenHh}";

    [DisplayName("Mã thể loại")]
    public virtual Loai? MaLoaiNavigation { get; set; } 

    [DisplayName("Mã nhà cung")]
    public virtual NhaCungCap? MaNccNavigation { get; set; } 

    public virtual ICollection<YeuThich>? YeuThiches { get; set; } = new List<YeuThich>();
}
