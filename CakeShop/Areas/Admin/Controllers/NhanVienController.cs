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
using CakeShop.ModelsView;

namespace CakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
    public class NhanVienController : Controller
    {
        private readonly CakeshopContext _context;

        public NhanVienController(CakeshopContext context)
        {
            _context = context;
        }

        // GET: Admin/NhanVien
        public async Task<IActionResult> Index()
        {
            return View(await _context.NhanViens.ToListAsync());
        }

        // GET: Admin/NhanVien/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // GET: Admin/NhanVien/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NhanVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNv,HoTen,Email,MatKhau")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                var tim = _context.NhanViens.Where(x => x.MaNv.Equals(nhanVien.MaNv)).ToList();
                if (tim.Any())
                {
                    ModelState.AddModelError("loi", "Đã có nhân viên này");
                    return View(nhanVien);
                }
                nhanVien.MatKhau = nhanVien.MatKhau.ToMd5Hash("4dm!n");
                _context.Add(nhanVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nhanVien);
        }

        // GET: Admin/NhanVien/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return View(nhanVien);
        }

        // POST: Admin/NhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("MaNv,HoTen,Email,MatKhau")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    nhanVien.MatKhau = nhanVien.MatKhau.ToMd5Hash("4dm!n");
                    _context.Entry(nhanVien).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(nhanVien.MaNv))
                    {
                        TempData["Message"] = "Nhân viên này không có trong hệ thống!";
                        return RedirectToAction("NotFound", "Admin/HangHoa");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nhanVien);
        }

        // POST: Admin/NhanVien/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id != null)
            {
                TempData["Message"] = "Không thể xóa sản phẩm này!";
            }
            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(m => m.MaNv == id);
            if (nhanVien == null)
            {
                TempData["Message"] = "Không tồn tại nhân viên này!";
            }
            else
            {
                var phanCong = _context.PhanCongs.Where(x => x.MaNv.Equals(id)).ToList();
                if (phanCong.Any())
                {
                    foreach (var ha in phanCong)
                    {
                        _context.PhanCongs.Remove(ha);
                    }
                    _context.SaveChanges();
                }
                _context.Remove(_context.NhanViens.Find(id));
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("NotFound", "Admin/HangHoa");
        }

        private bool NhanVienExists(string id)
        {
            return _context.NhanViens.Any(e => e.MaNv == id);
        }
    }
}
