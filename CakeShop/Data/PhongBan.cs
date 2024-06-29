using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CakeShop.Data;

public partial class PhongBan
{
    [DisplayName("Mã phòng ban")]
    public string MaPb { get; set; } = null!;
    [DisplayName("Tên phòng ban")]
    public string TenPb { get; set; } = null!;
    [DisplayName("Thông tin")]
    public string? ThongTin { get; set; }

    public virtual ICollection<PhanCong> PhanCongs { get; set; } = new List<PhanCong>();

    public virtual ICollection<PhanQuyen> PhanQuyens { get; set; } = new List<PhanQuyen>();
}
