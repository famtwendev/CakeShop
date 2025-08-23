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
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace CakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/HangHoa/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
    public class TheLoaiController : Controller
    {
        private readonly CakeshopContext _context;

        public TheLoaiController(CakeshopContext context)
        {
            _context = context;
        }

        // GET: Admin/TheLoai
        public async Task<IActionResult> Index()
        {
            return View(await _context.Loais.ToListAsync());
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                TempData["Message"] = "Không thể xóa sản phẩm này!";
            }

            var loai = await _context.Loais.FirstOrDefaultAsync(m => m.MaLoai == id);
            if (loai != null)
            {
                _context.Loais.Remove(loai);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "Không thể xóa sản phẩm này!";
            return Redirect("/Admin/HangHoa/NotFound");
        }

        // GET: Admin/TheLoai/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TheLoai/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLoai,TenLoai,TenLoaiAlias,MoTa,Hinh")] Loai loai)
        {
            if (ModelState.IsValid)
            {
                var tim = _context.Loais.Where(x => x.MaLoai.Equals(loai.MaLoai)).ToList();
                if (tim.Any())
                {
                    return View(loai);
                }
                _context.Add(loai);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loai);
        }

        // GET: Admin/TheLoai/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loai = await _context.Loais.FindAsync(id);
            if (loai == null)
            {
                return NotFound();
            }
            return View(loai);
        }

        // POST: Admin/TheLoai/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("MaLoai,TenLoai,TenLoaiAlias,MoTa,Hinh")] Loai loai)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(loai).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiExists(loai.MaLoai))
                    {
                        TempData["Message"] = "Không có thể loại này!";
                        return Redirect("/Admin/HangHoa/NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(loai);
        }

        private bool LoaiExists(int id)
        {
            return _context.Loais.Any(e => e.MaLoai == id);
        }
    }
}
