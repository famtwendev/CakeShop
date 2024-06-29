using AutoMapper;
using CakeShop.Areas.Admin.Data;
using CakeShop.Controllers;
using CakeShop.Data;
using CakeShop.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace CakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        // Login user:phamtuyenad
        // Login pass:admin4dm!n
        private readonly CakeshopContext db;
        private readonly IMapper _mapper;
        public AdminController(CakeshopContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        [Route("")]
        [Route("Index")]
        [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            else
                return View();
        }
        #region LoginSubmit
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(AdminLoginVM model)
        {
            if (ModelState.IsValid)
            {
                var nhanVien = db.NhanViens.SingleOrDefault(nv => nv.MaNv == model.UserName);
                if (nhanVien == null)
                {
                    ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                }
                else
                {
                    if (nhanVien.MatKhau != model.Password.ToMd5Hash("4dm!n"))//"4dm!n" là saltkey.ToMd5Hash("4dm!n")
                    {
                        ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                    }
                    else
                    {
                        var phancong = db.PhanCongs.FirstOrDefault(x => x.MaNv == nhanVien.MaNv);
                        if (phancong == null)
                        {
                            ModelState.AddModelError("loi", "Không tìm thấy thông tin phân công cho tài khoản này.");
                        }
                        if (phancong.HieuLuc != true)
                        {
                            ModelState.AddModelError("loi", "Tài khoản đã hết hiệu lực!");
                        }
                        else
                        {   // Sign out from customer authentication scheme if logged in
                            var claims = new List<Claim> {
                            new Claim(ClaimTypes.Name, nhanVien.HoTen),
                            new Claim(MySetting.CLAIM_ADMINID, nhanVien.MaNv),
                            new Claim(ClaimTypes.Role, SD.Role_Admin),
                            };
                            /*
                                                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                                                        await HttpContext.SignInAsync(claimsPrincipal);*/

                            var claimsIdentity = new ClaimsIdentity(claims, "AdminCookie");
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignOutAsync("CustomerCookie");
                            await HttpContext.SignInAsync("AdminCookie", claimsPrincipal);

                            return RedirectToAction("Index", "Admin", new { area = "Admin" });
                            /*return RedirectToAction("/Admin/Admin/Index");*/
                        }
                    }
                }
            }
            return View();
        }
        #endregion


        // GET: Admin/NhanVien/Edit/5
        [HttpGet]
        [Route("Profile")]
        [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
        public async Task<IActionResult> Profile()
        {
            var adminId = User.FindFirst("AdminID")?.Value;
            var nhanvien = db.NhanViens.SingleOrDefault(nv => nv.MaNv == adminId);
            if (nhanvien == null)
            {
                return NotFound();
            }
            return View(nhanvien);
        }

        // POST: Admin/NhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Profile")]
        [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
        public async Task<IActionResult> Profile(NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                nhanVien.MatKhau = nhanVien.MatKhau.ToMd5Hash("4dm!n");
                db.Entry(nhanVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(nhanVien);
        }

     #region Logout
        [Route("Logout")]
        [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            TempData["Message"] = "Đã đăng xuất quyền quản trị viên!";
            return Redirect("/Admin/ThongBao");
        }
        #endregion

        #region Alert
        [Route("NotFound")]
        public IActionResult NotFound()
        {
            return View();
        }
        [Route("ThongBao")]
        public IActionResult ThongBao()
        {
            return View();
        }
        #endregion
    }
}
