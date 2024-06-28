using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CakeShop.Data;

public partial class PhanCong
{
    [DisplayName("Mã phân công")]
    public int MaPc { get; set; }

    [DisplayName("Mã nhân viên")]
    public string MaNv { get; set; } = null!;

    [DisplayName("Mã phòng ban")]
    public string MaPb { get; set; } = null!;

    [DisplayName("Ngày phân công")]
    public DateTime NgayPc { get; set; }

    [DisplayName("Hiệu Lực")]
    public bool HieuLuc { get; set; }

    [DisplayName("Mã nhân viên")]
    public virtual NhanVien MaNvNavigation { get; set; } = null!;

    [DisplayName("Mã Phòng ban")]
    public virtual PhongBan MaPbNavigation { get; set; } = null!;
}
