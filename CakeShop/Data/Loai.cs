using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CakeShop.Data;

public partial class Loai
{
    [DisplayName("Mã Loại")]
    public int MaLoai { get; set; }

    [DisplayName("Tên Thể Loại")]
    public string TenLoai { get; set; } = null!;

    [DisplayName("Tên Loại Alias")]
    public string? TenLoaiAlias { get; set; }

    [DisplayName("Mô Tả")]
    public string? MoTa { get; set; }

    [DisplayName("Hình Ảnh")]
    public string? Hinh { get; set; }

    public virtual ICollection<HangHoa> HangHoas { get; set; } = new List<HangHoa>();

    public string DisplayText
    {
        get { return $"{MaLoai} ~ {TenLoai}"; }
    }
}
