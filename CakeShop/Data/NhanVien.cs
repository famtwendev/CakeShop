using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CakeShop.Data;

public partial class NhanVien
{
    [DisplayName("Mã nhân viên")]
    public string MaNv { get; set; } = null!;
    [DisplayName("Họ và tên")]
    public string HoTen { get; set; } = null!;
    [DisplayName("Email")]
    public string Email { get; set; } = null!;

    [DisplayName("Mật khẩu")]
    public string? MatKhau { get; set; }

    public virtual ICollection<ChuDe> ChuDes { get; set; } = new List<ChuDe>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ICollection<HoiDap> HoiDaps { get; set; } = new List<HoiDap>();

    public virtual ICollection<PhanCong> PhanCongs { get; set; } = new List<PhanCong>();
}
