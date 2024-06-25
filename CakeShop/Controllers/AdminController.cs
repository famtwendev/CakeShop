using CakeShop.Data;
using CakeShop.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using CakeShop.ModelsView.Admin;

namespace CakeShop.Controllers
{
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

        [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
        public IActionResult Index()
        {
            var adminId = User.FindFirst("AdminID")?.Value;

            var nhanVien = db.NhanViens.SingleOrDefault(nv => nv.MaNv == adminId);

            if (nhanVien == null)
            {
                return NotFound();
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        #region LoginSubmit
        [HttpPost]
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
                    if (nhanVien.MatKhau != model.Password)//"4dm!n" là saltkey.ToMd5Hash("4dm!n")
                    {
                        ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                    }
                    else
                    {
                        var phancong = db.PhanCongs.SingleOrDefault(pc => pc.MaNv == nhanVien.MaNv);
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
                            await HttpContext.SignInAsync("AdminCookie",claimsPrincipal);


                            return RedirectToAction("Index", "Admin");

                        }
                    }
                }
            }
            return View();
        }
        #endregion

        #region Logout
        [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            TempData["Message"] = "Đã đăng xuất quyền quản trị viên!";
            return Redirect("/ThongBao");
        }
        #endregion


        #region DanhMuc
        public IActionResult DanhMuc()
        {
            return View(db.Loais.ToList());// 
        }

        #endregion


        #region NhanSu
        public IActionResult NhanSu()
        {
            return View(db.NhanViens.ToList());// 
        }

        #endregion
    }
}
