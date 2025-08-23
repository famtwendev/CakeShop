using CakeShop.Data;
using CakeShop.ModelsView;
using Microsoft.AspNetCore.Mvc;

namespace CakeShop.ViewComponents
{
    public class PopularProductsViewComponent : ViewComponent
    {
        private readonly CakeshopContext db;
        public PopularProductsViewComponent(CakeshopContext context) => db = context;
    
        public IViewComponentResult Invoke()
        {
            // Select Loai 
            var data = db.HangHoas.Select(lo => new ChiTietHangHoaVM
            {
                MaLoai = lo.MaLoai,
                MaHh = lo.MaHh,
                TenLoai = db.Loais
                    .Where(lh => lh.MaLoai == lo.MaLoai)
                    .Select(lh => lh.TenLoai)
                    .FirstOrDefault(),
                TenHH = lo.TenHh,
                Hinh = lo.Hinh,
                DonGia = lo.DonGia,
                GiamGia = lo.GiamGia,
                HinhanhSps = lo.HinhanhSps,
            }).OrderBy(p => p.TenLoai);
            return View("TagProduct",data);
        }
    }
}
