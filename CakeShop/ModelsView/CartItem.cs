namespace CakeShop.ModelsView
{
    public class CartItem
    {
        public int MaHh { get; set; }
        public string Hinh { get; set; }
        public string TenHH { get; set; }
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public double GiamGia { get; set; }
        public double ThanhTien => SoLuong * (DonGia-GiamGia); 
    }

}
