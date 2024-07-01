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
using X.PagedList;
using CakeShop.ModelsView;

namespace CakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
    public class HoaDonController : Controller
    {
        private readonly CakeshopContext _context;

        public HoaDonController(CakeshopContext context)
        {
            _context = context;
        }

        // GET: Admin/HoaDon
        public IActionResult Index(int? page)
        {
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var cakeshopContext = _context.HoaDons.Include(h => h.MaKhNavigation).Include(h => h.MaNvNavigation).Include(h => h.MaTrangThaiNavigation).ToPagedList(pageNumber, pageSize); ;
            return View(cakeshopContext);
        }

        // GET: Admin/HoaDon/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaNvNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // GET: Admin/HoaDon/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", hoaDon.MaKh);
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", hoaDon.MaNv);
            ViewData["MaTrangThai"] = new SelectList(_context.TrangThais, "MaTrangThai", "MaTrangThai", hoaDon.MaTrangThai);
            return View(hoaDon);
        }

        // POST: Admin/HoaDon/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("MaHd,MaKh,NgayDat,NgayCan,NgayGiao,HoTen,DiaChi,DienThoai,CachThanhToan,CachVanChuyen,PhiVanChuyen,MaTrangThai,MaNv,GhiChu")] HoaDon hoaDon)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    hoaDon.MaKhNavigation = hoaDon.MaKhNavigation;
                    _context.Entry(hoaDon).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.MaHd))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", hoaDon.MaKh);
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", hoaDon.MaNv);
            ViewData["MaTrangThai"] = new SelectList(_context.TrangThais, "MaTrangThai", "MaTrangThai", hoaDon.MaTrangThai);
            return View(hoaDon);
        }

        // POST: Admin/HoaDon/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                TempData["Message"] = "Không thể xóa hóa đơn này!";
            }
            var hoaDon = _context.HoaDons.FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoaDon == null)
            {
                TempData["Message"] = "Không tồn tại hóa đơn này!";
            }
            else
            {
                var chiTietHoaDon = _context.ChiTietHds.Where(x => x.MaHd.Equals(id)).ToList();
                if (chiTietHoaDon.Any())
                {
                    foreach (var ha in chiTietHoaDon)
                    {
                        _context.ChiTietHds.Remove(ha);
                    }
                    _context.SaveChanges();
                }
                _context.HoaDons.Remove(_context.HoaDons.Find(id));
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return Redirect("/Admin/HangHoa/NotFound");
        }

        private bool HoaDonExists(int id)
        {
            return _context.HoaDons.Any(e => e.MaHd == id);
        }
    }
}
