using CakeShop.Data;
using CakeShop.ModelsView;
using Microsoft.AspNetCore.Mvc;

namespace CakeShop.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly CakeshopContext db;
        public MenuLoaiViewComponent(CakeshopContext context) => db = context;
    
        public IViewComponentResult Invoke()
        {
            // Select Loai 
            var data = db.Loais.Select(lo => new MenuLoaiVM
            {
                MaLoai = lo.MaLoai,
                TenLoai =  lo.TenLoai, 
                SoLuong =  lo.HangHoas.Count,
            }).OrderBy(p => p.TenLoai);
            return View("Default",data);
        }
    }
}
