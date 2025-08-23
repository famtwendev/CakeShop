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
using CakeShop.Areas.Admin.Models;
using CakeShop.ModelsView;

namespace CakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/HangHoa/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
    public class HinhanhController : Controller
    {
        private readonly CakeshopContext _context;

        public HinhanhController(CakeshopContext context)
        {
            _context = context;
        }

        // GET: Admin/Hinhanh
        public IActionResult Index(int? page)
        {
            int pageSize = 6;
            int pageNumber = page ?? 1;
            var cakeshopContext = _context.HinhanhSps.Include(h => h.MaHhNavigation).ToPagedList(pageNumber, pageSize);
            return View(cakeshopContext);
        }

        // GET: Admin/Hinhanh/Details/5
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

        // GET: Admin/Hinhanh/Create
        public async Task<IActionResult> Create()
        {
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHhTenHh");
            return View();
        }

        // POST: Admin/Hinhanh/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.[Bind("Id,MaHh,HinhAnhPhu")] 
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(HinhanhSp hinhanhSp, IFormFile HinhAnhPhu)
        {
            if (hinhanhSp.Id != null)
            {
                if (HinhAnhPhu != null && HinhAnhPhu.Length > 0)
                {
                    // Danh sách các đuôi tệp hợp lệ
                    var permittedExtensions = new[] { ".png", ".jpg", ".jpeg" };

                    // Lấy đuôi tệp
                    var ext = Path.GetExtension(HinhAnhPhu.FileName).ToLowerInvariant();

                    // Kiểm tra đuôi tệp
                    if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                    {
                        ModelState.AddModelError("HinhAnhPhu", "Chỉ cho phép tệp hình ảnh có đuôi .png, .jpg, .jpeg.");
                        ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHhTenHh", hinhanhSp.MaHh);
                        return View(hinhanhSp);
                    }

                    // Kiểm tra kiểu MIME
                    var mimeType = HinhAnhPhu.ContentType.ToLower();
                    if (!mimeType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("HinhAnhPhu", "Tệp không phải là hình ảnh.");
                        ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHhTenHh", hinhanhSp.MaHh);
                        return View(hinhanhSp);
                    }

                    // Nếu tệp hợp lệ, lưu tệp
                    hinhanhSp.HinhAnhPhu = Path.GetFileName(HinhAnhPhu.FileName);
                }
                _context.Add(hinhanhSp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHhTenHh", hinhanhSp.MaHh);
            return View(hinhanhSp);
        }

        // GET: Admin/Hinhanh/Edit/5
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
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHhTenHh", hinhanhSp.MaHh);
            return View(hinhanhSp);
        }

        // POST: Admin/Hinhanh/Edit/5/[Bind("Id,MaHh,HinhAnhPhu")]
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HinhanhSp hinhanhSp, IFormFile HinhAnhPhu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (HinhAnhPhu != null && HinhAnhPhu.Length > 0)
                    {
                        // Danh sách các đuôi tệp hợp lệ
                        var permittedExtensions = new[] { ".png", ".jpg", ".jpeg" };

                        // Lấy đuôi tệp
                        var ext = Path.GetExtension(HinhAnhPhu.FileName).ToLowerInvariant();

                        // Kiểm tra đuôi tệp
                        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                        {
                            ModelState.AddModelError("HinhAnhPhu", "Chỉ cho phép tệp hình ảnh có đuôi .png, .jpg, .jpeg.");
                            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHhTenHh", hinhanhSp.MaHh);
                            return View(hinhanhSp);
                        }

                        // Kiểm tra kiểu MIME
                        var mimeType = HinhAnhPhu.ContentType.ToLower();
                        if (!mimeType.StartsWith("image/"))
                        {
                            ModelState.AddModelError("HinhAnhPhu", "Tệp không phải là hình ảnh.");
                            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHhTenHh", hinhanhSp.MaHh);
                            return View(hinhanhSp);
                        }

                        // Nếu tệp hợp lệ, lưu tệp
                        hinhanhSp.HinhAnhPhu = Path.GetFileName(HinhAnhPhu.FileName);
                    }
                    _context.Entry(hinhanhSp).State = EntityState.Modified;
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
            ViewData["MaHh"] = new SelectList(_context.HangHoas, "MaHh", "MaHhTenHh", hinhanhSp.MaHh);
            return View(hinhanhSp);
        }
        // POST: Admin/Hinhanh/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                TempData["Message"] = "Không thể xóa hình ảnh này!";
            }
            var hinhanhSp = await _context.HinhanhSps.FirstOrDefaultAsync(m => m.Id == id);
            if (hinhanhSp != null)
            {
                _context.HinhanhSps.Remove(hinhanhSp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "Không thể xóa sản phẩm này!";
            return Redirect("/Admin/HangHoa/NotFound");
        }

        private bool HinhanhSpExists(int id)
        {
            return _context.HinhanhSps.Any(e => e.Id == id);
        }
    }
}
