using CakeShop.Data;

namespace CakeShop.ModelsView
{
    public class HangHoaVM
    {
        public int MaHh { get; set; }

        public string TenHH { get; set; }

        public string Hinh { get; set; }

        public double DonGia { get; set; }

        public string MoTaNgan { get; set; }

        public string TenLoai { get; set; }

        public double Giamgia { get; set; }
    }

    public class ChiTietHangHoaVM
    {
        public int MaHh { get; set; }
        public string TenHH { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public double GiamGia { get; set; }
        public string MoTaNgan { get; set; }
        public int MaLoai { get; set; }
        public string TenLoai { get; set; }
        public string ChiTiet { get; set; }
        public int DiemDanhGia { get; set; }
        public int SoLuongTon { get; set; }
        public ICollection<HinhanhSp> HinhanhSps { get; set; } = new List<HinhanhSp>();
    }
}
