using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CakeShop.Data;

public partial class NhaCungCap
{
    [DisplayName("Mã nhà cung")]
    public string MaNcc { get; set; } = null!;

    [DisplayName("Tên công ty")]
    public string TenCongTy { get; set; } = null!;

    [DisplayName("Logo")]
    public string Logo { get; set; } = null!;

    [DisplayName("Người đại diện")]
    public string? NguoiLienLac { get; set; }

    [DisplayName("Email")]
    public string Email { get; set; } = null!;

    [DisplayName("Số điện thoại")]
    public string? DienThoai { get; set; }

    [DisplayName("Địa chỉ")]
    public string? DiaChi { get; set; }

    [DisplayName("Mô tả")]
    public string? MoTa { get; set; }

    public virtual ICollection<HangHoa> HangHoas { get; set; } = new List<HangHoa>();
}
