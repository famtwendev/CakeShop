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

namespace CakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/HoaDon/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
    public class ChiTietHoadonController : Controller
    {
        private readonly CakeshopContext _context;

        public ChiTietHoadonController(CakeshopContext context)
        {
            _context = context;
        }

        // GET: Admin/ChiTietHoadon
        public async Task<IActionResult> Index()
        {
            var cakeshopContext = _context.ChiTietHds.Include(c => c.MaHdNavigation).Include(c => c.MaHhNavigation);
            return View(await cakeshopContext.ToListAsync());
        }

        // GET: Admin/ChiTietHoadon/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietHd = await _context.ChiTietHds
                .Include(c => c.MaHdNavigation)
                .Include(c => c.MaHhNavigation)
                .FirstOrDefaultAsync(m => m.MaCt == id);
            if (chiTietHd == null)
            {
                return NotFound();
            }

            return View(chiTietHd);
        }

        // GET: Admin/ChiTietHoadon/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietHd = await _context.ChiTietHds.FindAsync(id);
            if (chiTietHd == null)
            {
                return NotFound();
            }
            ViewData["MaHd"] = new SelectList(_context.HoaDons, "MaHd", "MaHd", chiTietHd.MaHd);
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHh", chiTietHd.MaHh);
            return View(chiTietHd);
        }

        // POST: Admin/ChiTietHoadon/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("MaCt,MaHd,MaHh,DonGia,SoLuong,GiamGia")] ChiTietHd chiTietHd)
        {
          if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(chiTietHd).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTietHdExists(chiTietHd.MaCt))
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
            ViewData["MaHd"] = new SelectList(_context.HoaDons, "MaHd", "MaHd", chiTietHd.MaHd);
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHh", chiTietHd.MaHh);
            return View(chiTietHd);
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                TempData["Message"] = "Không thể xóa chi tiết hóa đơn này!";
            }
            var chitiethd = _context.ChiTietHds.FirstOrDefault(m => m.MaCt == id);
            if (chitiethd != null)
            {
                _context.ChiTietHds.Remove(chitiethd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "Không thể xóa chi tiết hóa đơn này!";
            return Redirect("/Admin/HangHoa/NotFound");
        }

        private bool ChiTietHdExists(int id)
        {
            return _context.ChiTietHds.Any(e => e.MaCt == id);
        }
    }
}
