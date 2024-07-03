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
    public class KhachHangController : Controller
    {
        private readonly CakeshopContext _context;

        public KhachHangController(CakeshopContext context)
        {
            _context = context;
        }

        // GET: Admin/KhachHang
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 4;
            int pageNumber = page ?? 1;
            var cakeshopContext = _context.KhachHangs.ToPagedList(pageNumber, pageSize);
            return View(cakeshopContext);
        }

        // GET: Admin/KhachHang/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // GET: Admin/KhachHang/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/KhachHang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKh,MatKhau,HoTen,GioiTinh,NgaySinh,DiaChi,DienThoai,Email,Hinh,HieuLuc,VaiTro,RandomKey")] KhachHang khachHang)
        {
			if (ModelState.IsValid)
			{
				// Kiểm tra xem MaKh đã tồn tại chưa
				var existingMaKh = await _context.KhachHangs.FirstOrDefaultAsync(x => x.MaKh == khachHang.MaKh);
				if (existingMaKh != null)
				{
					ModelState.AddModelError("MaKh", "Mã khách hàng đã tồn tại");
					return View(khachHang);
				}

				// Kiểm tra xem Email đã tồn tại chưa
				var existingEmail = await _context.KhachHangs.FirstOrDefaultAsync(x => x.Email == khachHang.Email);
				if (existingEmail != null)
				{
					ModelState.AddModelError("Email", "Email đã được sử dụng bởi tài khoản khác");
					return View(khachHang);
				}

				// Generate random key and hash password
				khachHang.RandomKey = MyUtil.GenerateRamdomKey();
				khachHang.MatKhau = khachHang.MatKhau.ToMd5Hash(khachHang.RandomKey);

				// Add the new KhachHang to the context
				_context.KhachHangs.Add(khachHang);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}
			return View(khachHang);
		}

        // GET: Admin/KhachHang/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return View(khachHang);
        }

        // POST: Admin/KhachHang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("MaKh,MatKhau,HoTen,GioiTinh,NgaySinh,DiaChi,DienThoai,Email,Hinh,HieuLuc,VaiTro,RandomKey")] KhachHang khachHang)
        {          
            if (ModelState.IsValid)
            {
                try
                {
					khachHang.RandomKey = MyUtil.GenerateRamdomKey();
					khachHang.MatKhau = khachHang.MatKhau.ToMd5Hash(khachHang.RandomKey);
					_context.Entry(khachHang).State = EntityState.Modified;
					_context.Update(khachHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangExists(khachHang.MaKh))
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
            return View(khachHang);
        }

        // POST: Admin/KhachHang/Delete/
        public async Task<IActionResult> Delete(string id)
        {
			if (id == null)
			{
				ModelState.AddModelError("loi", "Không thể xóa sản phẩm này!");
				return View("Index", "KhachHang");
			}
			var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang != null)
            {
                _context.KhachHangs.Remove(khachHang); 
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
			return View("Index", "KhachHang");
		}

        private bool KhachHangExists(string id)
        {
            return _context.KhachHangs.Any(e => e.MaKh == id);
        }
    }
}
