using CakeShop.Data;
using CakeShop.Models;
using CakeShop.ModelsView;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace EcommerceWeb.Controllers
{
    public class HangHoaController : Controller
    {
        public readonly CakeshopContext db;
        public HangHoaController(CakeshopContext context)
        {
            db = context;
        }
        

        public IActionResult Index(int? loai, int? page)
        {

            var hangHoas = db.HangHoas.AsQueryable();
            int pageSize = 6;
            int pageNumber = page ?? 1;
            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value); 
                ViewBag.Loai = loai; // Truyền giá trị loai sang ViewBag
            }
            
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai,
                Giamgia = p.GiamGia,
            }).ToPagedList(pageNumber, pageSize);

            return View(result);
            /*            var hangHoas = db.HangHoas.AsQueryable();
                        if (loai.HasValue)
                        {
                            hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
                        }
                        var result = hangHoas.Select(p => new HangHoaVM
                        {
                            MaHh = p.MaHh,
                            TenHH = p.TenHh,
                            DonGia = p.DonGia,
                            Hinh = p.Hinh ?? "",
                            MoTaNgan = p.MoTaDonVi ?? "",
                            TenLoai = p.MaLoaiNavigation.TenLoai,
                            Giamgia = p.GiamGia,
                        });
                        return View(result);*/
        }

        public IActionResult Search(string? query, int? page)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            int pageSize = 6;
            int pageNumber = page ?? 1;

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            }).ToPagedList(pageNumber, pageSize);

            return View(result);
            /*var hangHoas = db.HangHoas.AsQueryable();

            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            return View(result);*/
        }
        public IActionResult Detail(int id)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .Include(p => p.HinhanhSps) // Include the related HinhanhSps // 
                .SingleOrDefault(p => p.MaHh == id);
            if (data == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }

            var result = new ChiTietHangHoaVM
            {
                MaHh = data.MaHh,
                TenHH = data.TenHh,
                DonGia = data.DonGia,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,
                MaLoai = data.MaLoai,
                GiamGia = data.GiamGia,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10,//tính sau
                DiemDanhGia = 5,//check sau
                HinhanhSps = data.HinhanhSps?.Select(h => new HinhanhSp
                {
                    Id = h.Id,
                    MaHh = h.MaHh,
                    HinhAnhPhu = h.HinhAnhPhu
                }).ToList() ?? new List<HinhanhSp>()
            };
            return View(result);
        }
    }
}
