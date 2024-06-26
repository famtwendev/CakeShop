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
    [Route("Admin/HangHoas/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
    public class HinhanhSpsController : Controller
    {
        private readonly CakeshopContext _context;

        public HinhanhSpsController(CakeshopContext context)
        {
            _context = context;
        }

        // GET: Admin/HinhanhSps
        public async Task<IActionResult> Index()
        {
            var cakeshopContext = _context.HinhanhSps.Include(h => h.MaHhNavigation);
            return View(await cakeshopContext.ToListAsync());
        }

        // GET: Admin/HinhanhSps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hinhanhSp = await _context.HinhanhSps
                .Include(h => h.MaHhNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hinhanhSp == null)
            {
                return NotFound();
            }

            return View(hinhanhSp);
        }

        // GET: Admin/HinhanhSps/Create
        public IActionResult Create()
        {
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHh");
            return View();
        }

        // POST: Admin/HinhanhSps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaHh,HinhAnhPhu")] HinhanhSp hinhanhSp)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hinhanhSp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHh", hinhanhSp.MaHh);
            return View(hinhanhSp);
        }

        // GET: Admin/HinhanhSps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hinhanhSp = await _context.HinhanhSps.FindAsync(id);
            if (hinhanhSp == null)
            {
                return NotFound();
            }
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHh", hinhanhSp.MaHh);
            return View(hinhanhSp);
        }

        // POST: Admin/HinhanhSps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaHh,HinhAnhPhu")] HinhanhSp hinhanhSp)
        {
            if (id != hinhanhSp.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hinhanhSp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HinhanhSpExists(hinhanhSp.Id))
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
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHh", hinhanhSp.MaHh);
            return View(hinhanhSp);
        }

        // GET: Admin/HinhanhSps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hinhanhSp = await _context.HinhanhSps
                .Include(h => h.MaHhNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hinhanhSp == null)
            {
                return NotFound();
            }

            return View(hinhanhSp);
        }

        // POST: Admin/HinhanhSps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hinhanhSp = await _context.HinhanhSps.FindAsync(id);
            if (hinhanhSp != null)
            {
                _context.HinhanhSps.Remove(hinhanhSp);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HinhanhSpExists(int id)
        {
            return _context.HinhanhSps.Any(e => e.Id == id);
        }
    }
}
