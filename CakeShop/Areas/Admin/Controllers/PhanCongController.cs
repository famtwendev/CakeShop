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
using Microsoft.AspNetCore.Components.Forms;

namespace CakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/NhanVien/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
    public class PhanCongController : Controller
    {
        private readonly CakeshopContext _context;

        public PhanCongController(CakeshopContext context)
        {
            _context = context;
        }

        // GET: Admin/PhanCong
        public async Task<IActionResult> Index()
        {
            var cakeshopContext = _context.PhanCongs.Include(p => p.MaNvNavigation).Include(p => p.MaPbNavigation);
            return View(await cakeshopContext.ToListAsync());
        }

        // GET: Admin/PhanCong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanCong = await _context.PhanCongs
                .Include(p => p.MaNvNavigation)
                .Include(p => p.MaPbNavigation)
                .FirstOrDefaultAsync(m => m.MaPc == id);
            if (phanCong == null)
            {
                return NotFound();
            }

            return View(phanCong);
        }

        // GET: Admin/PhanCong/Create
        public IActionResult Create(int? id)
        {
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv");
            ViewData["MaPb"] = new SelectList(_context.PhongBans, "MaPb", "MaPb");
            return View();
        }

        // POST: Admin/PhanCong/CreateModelState.IsValid
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(PhanCong phanCong)
        {
            if (phanCong.MaPc != null)
            {
                try
                {
                    _context.PhanCongs.Add(phanCong);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. " + ex.Message);
                }
            }
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", phanCong.MaNv);
            ViewData["MaPb"] = new SelectList(_context.PhongBans, "MaPb", "MaPb", phanCong.MaPb);
            return View(phanCong);
        }

        // GET: Admin/PhanCong/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var phanCong = await _context.PhanCongs.FindAsync(id);
            if (phanCong == null)
            {
                return NotFound();
            }
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", phanCong.MaNv);
            ViewData["MaPb"] = new SelectList(_context.PhongBans, "MaPb", "MaPb", phanCong.MaPb);
            return View(phanCong);
        }

        // POST: Admin/PhanCong/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("MaPc,MaNv,MaPb,NgayPc,HieuLuc")] PhanCong phanCong)
        {
            if (phanCong.MaPc != null)
            {

                try
                {
                    _context.Entry(phanCong).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhanCongExists(phanCong.MaPc))
                    {
                        TempData["Message"] = "Lỗi phân công!";
                        return Redirect("/Admin/HangHoa/NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "MaNv", phanCong.MaNv);
            ViewData["MaPb"] = new SelectList(_context.PhongBans, "MaPb", "MaPb", phanCong.MaPb);
            return View(phanCong);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                TempData["Message"] = "Không có mã phân công!";
            }
            var phanCong = await _context.PhanCongs.FirstOrDefaultAsync(m => m.MaPc == id);
            if (phanCong != null)
            {
                _context.PhanCongs.Remove(phanCong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "Không thể xóa phân công này!";
            return Redirect("/Admin/HangHoa/NotFound");
        }

        private bool PhanCongExists(int id)
        {
            return _context.PhanCongs.Any(e => e.MaPc == id);
        }
    }
}
