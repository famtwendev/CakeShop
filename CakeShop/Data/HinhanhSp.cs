using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CakeShop.Data;

public partial class HinhanhSp
{
    public int Id { get; set; }

    [DisplayName("Mã hàng hóa")]
    public int? MaHh { get; set; }

    [DisplayName("Hình ảnh phụ")]
    public string? HinhAnhPhu { get; set; }

    public virtual HangHoa? MaHhNavigation { get; set; }
}
