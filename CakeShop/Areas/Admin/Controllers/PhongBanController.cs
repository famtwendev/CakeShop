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
    [Route("Admin/NhanVien/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
    public class PhongBanController : Controller
    {
        private readonly CakeshopContext _context;

        public PhongBanController(CakeshopContext context)
        {
            _context = context;
        }

        // GET: Admin/PhongBan
        public async Task<IActionResult> Index()
        {
            return View(await _context.PhongBans.ToListAsync());
        }

        // GET: Admin/PhongBan/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phongBan = await _context.PhongBans
                .FirstOrDefaultAsync(m => m.MaPb == id);
            if (phongBan == null)
            {
                return NotFound();
            }

            return View(phongBan);
        }

        // GET: Admin/PhongBan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PhongBan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaPb,TenPb,ThongTin")] PhongBan phongBan)
        {
            if (ModelState.IsValid)
            {
                _context.PhongBans.Add(phongBan);
                // Save changes to the database
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(phongBan);
        }

        // GET: Admin/PhongBan/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phongBan = await _context.PhongBans.FindAsync(id);
            if (phongBan == null)
            {
                return NotFound();
            }
            return View(phongBan);
        }

        // POST: Admin/PhongBan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("MaPb,TenPb,ThongTin")] PhongBan phongBan)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(phongBan).State = EntityState.Modified;
/*                    _context.Update(phongBan);*/
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongBanExists(phongBan.MaPb))
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
            return View(phongBan);
        }


        // POST: Admin/PhongBan/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                TempData["Message"] = "Không thể xóa sản phẩm này!";
            }

            var phongban = await _context.PhongBans.FirstOrDefaultAsync(m => m.MaPb == id);
            if (phongban != null)
            {
                _context.PhongBans.Remove(phongban);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "Không thể xóa sản phẩm này!";
            return Redirect("/Admin/HangHoa/NotFound");
            /*if (id == null)
            {
                TempData["Message"] = "Không thể xóa phòng ban này!";
            }
            var phongBan = await _context.PhongBans.FindAsync(id);
            if (phongBan != null)
            {
                _context.PhongBans.Remove(phongBan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "Không thể xóa phòng ban này!";
            return Redirect("/Admin/HangHoa/NotFound");*/
        }

        private bool PhongBanExists(string id)
        {
            return _context.PhongBans.Any(e => e.MaPb == id);
        }
    }
}
