using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CakeShop.Data;
using CakeShop.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Drawing.Printing;
using X.PagedList;
using CakeShop.Areas.Admin.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
    public class HangHoaController : Controller
    {
        private readonly CakeshopContext _context;

        public HangHoaController(CakeshopContext context)
        {
            _context = context;
        }

        // GET: Admin/HangHoa
        public IActionResult Index(int? page)
        {
            int pageSize = 4;
            int pageNumber = page ?? 1;
            var cakeshopContext = _context.HangHoas.Include(h => h.MaLoaiNavigation).Include(h => h.MaNccNavigation).ToPagedList(pageNumber, pageSize);
            return View(cakeshopContext);
        }

        // GET: Admin/HangHoa/Details/5
        public IActionResult Details(int? id)
        {
            var data = _context.HangHoas
               .Include(p => p.MaLoaiNavigation)
               .Include(p => p.HinhanhSps) // Include the related HinhanhSps
               .SingleOrDefault(p => p.MaHh == id);
            if (data == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return NotFound();
            }

            var result = new ChiTietHangHoa
            {
                MaHh = data.MaHh,
                TenHh = data.TenHh,
                TenAlias = data.MaLoaiNavigation.TenLoai,
                DonGia = data.DonGia,
                MaLoai = data.MaLoai,
                MoTaDonVi = data.MoTaDonVi ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                NgaySx = data.NgaySx,
                GiamGia = data.GiamGia,
                SoLanXem = data.SoLanXem,
                MoTa = data.MoTa ?? string.Empty,
                MaNcc = data.MaNcc,
                HinhanhSps = data.HinhanhSps?.Select(h => new HinhanhSp
                {
                    Id = h.Id,
                    MaHh = h.MaHh,
                    HinhAnhPhu = h.HinhAnhPhu
                }).ToList() ?? new List<HinhanhSp>()
            };
            return View(result);
        }

        // GET: Admin/HangHoa/Create
        public IActionResult Create()
        {
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "DisplayText");
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc");
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHh");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChiTietHangHoa model, IFormFile Hinh)
        {
            var cthangHoa = new ChiTietHangHoa();
            if (ModelState.IsValid)
            {
                var tim = _context.HangHoas.Where(x => x.MaHh.Equals(model.MaHh)).ToList();
                if (tim.Any())
                {
                    ModelState.AddModelError("loi", "Đã có nhà cung cấp này");
                    return View(model);
                }
                var hangHoa = new HangHoa();
                hangHoa.TenHh = model.TenHh;
                hangHoa.TenAlias = model.TenAlias;
                hangHoa.MaLoai = model.MaLoai;
                hangHoa.MoTaDonVi = model.MoTaDonVi;
                hangHoa.DonGia = model.DonGia;
                hangHoa.GiamGia = model.GiamGia;
                hangHoa.SoLanXem = model.SoLanXem;
                hangHoa.MoTa = model.MoTa;
                hangHoa.MaNcc = model.MaNcc;
                if (Hinh != null && Hinh.Length > 0)
                {
                    /*string temp = MyUtil.UploadHinh(Hinh, "HangHoa");*/
                    hangHoa.Hinh = Path.GetFileName(Hinh.FileName);
                }
                hangHoa.NgaySx = model.NgaySx;
                _context.HangHoas.Add((hangHoa));
                // Save changes to the database
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai", cthangHoa.MaLoai);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc", cthangHoa.MaNcc);
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHh", cthangHoa.MaHh);
            return View(cthangHoa);
        }

        // GET: Admin/HangHoa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Message"] = "Không tìm thấy mã hàng hóa!";
                return NotFound();
            }

            var hangHoa = await _context.HangHoas.FindAsync(id);
            if (hangHoa == null)
            {
                return NotFound();
            }
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai", hangHoa.MaLoai);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc", hangHoa.MaNcc);
            return View(hangHoa);
        }

        // POST: Admin/HangHoa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("MaHh,TenHh,TenAlias,MaLoai,MoTaDonVi,DonGia,Hinh,NgaySx,GiamGia,SoLanXem,MoTa,MaNcc")] HangHoa hangHoa)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(hangHoa).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HangHoaExists(hangHoa.MaHh))
                    {
                        TempData["Message"] = "Mặt hàng này không có trong hệ thống!";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hangHoa);
        }

        // POST: Admin/HangHoa/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                TempData["Message"] = "Không thể xóa sản phẩm này!";
                return NotFound();
            }
            var sanPham = await _context.HangHoas.FirstOrDefaultAsync(m => m.MaHh == id);
            if (sanPham == null)
            {
                TempData["Message"] = "Không tồn tại sản phẩm này!";
                return NotFound();
            }

            var hinhanhPhu = _context.HinhanhSps.Where(x => x.MaHh.Equals(id)).ToList();
            if (hinhanhPhu.Any())
            {
                foreach (var ha in hinhanhPhu)
                {

                    /*                            MyUtil.DeleteHinh(ha.HinhAnhPhu, "HinhAnhSp");*/
                    _context.HinhanhSps.Remove(ha);
                }
                await _context.SaveChangesAsync();
            }
            /*MyUtil.DeleteHinh(_context.HangHoas.Find(id).Hinh.ToString(), "HangHoa");*/
            _context.HangHoas.Remove(sanPham);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult PhanTicHSale()
        {
            return View();
        }



        private bool HangHoaExists(int id)
        {
            return _context.HangHoas.Any(e => e.MaHh == id);
        }

        [Route("/404")]
        public IActionResult NotFound()
        {
            return View();
        }
    }
}
