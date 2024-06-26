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
    [Route("Admin/KhachHangs/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
    public class YeuThichesController : Controller
    {
        private readonly CakeshopContext _context;

        public YeuThichesController(CakeshopContext context)
        {
            _context = context;
        }

        // GET: Admin/YeuThiches
        public async Task<IActionResult> Index()
        {
            var cakeshopContext = _context.YeuThiches.Include(y => y.MaHhNavigation).Include(y => y.MaKhNavigation);
            return View(await cakeshopContext.ToListAsync());
        }

        // GET: Admin/YeuThiches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yeuThich = await _context.YeuThiches
                .Include(y => y.MaHhNavigation)
                .Include(y => y.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaYt == id);
            if (yeuThich == null)
            {
                return NotFound();
            }

            return View(yeuThich);
        }

        // GET: Admin/YeuThiches/Create
        public IActionResult Create()
        {
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHh");
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh");
            return View();
        }

        // POST: Admin/YeuThiches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaYt,MaHh,MaKh,NgayChon,MoTa")] YeuThich yeuThich)
        {
            if (ModelState.IsValid)
            {
                _context.Add(yeuThich);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHh", yeuThich.MaHh);
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", yeuThich.MaKh);
            return View(yeuThich);
        }

        // GET: Admin/YeuThiches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yeuThich = await _context.YeuThiches.FindAsync(id);
            if (yeuThich == null)
            {
                return NotFound();
            }
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHh", yeuThich.MaHh);
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", yeuThich.MaKh);
            return View(yeuThich);
        }

        // POST: Admin/YeuThiches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaYt,MaHh,MaKh,NgayChon,MoTa")] YeuThich yeuThich)
        {
            if (id != yeuThich.MaYt)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(yeuThich);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!YeuThichExists(yeuThich.MaYt))
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
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHh", yeuThich.MaHh);
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", yeuThich.MaKh);
            return View(yeuThich);
        }

        // GET: Admin/YeuThiches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yeuThich = await _context.YeuThiches
                .Include(y => y.MaHhNavigation)
                .Include(y => y.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaYt == id);
            if (yeuThich == null)
            {
                return NotFound();
            }

            return View(yeuThich);
        }

        // POST: Admin/YeuThiches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yeuThich = await _context.YeuThiches.FindAsync(id);
            if (yeuThich != null)
            {
                _context.YeuThiches.Remove(yeuThich);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool YeuThichExists(int id)
        {
            return _context.YeuThiches.Any(e => e.MaYt == id);
        }
    }
}
