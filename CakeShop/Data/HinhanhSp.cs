using System;
using System.Collections.Generic;

namespace CakeShop.Data;

public partial class HinhanhSp
{
    public int Id { get; set; }

    public int? MaHh { get; set; }

    public string? HinhAnhPhu { get; set; }

    public virtual HangHoa? MaHhNavigation { get; set; }
}
