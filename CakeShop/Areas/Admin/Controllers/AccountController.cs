using AutoMapper;
using CakeShop.Areas.Admin.Data;
using CakeShop.Controllers;
using CakeShop.Data;
using CakeShop.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace CakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class AccountController : Controller
    {
        // Login user:phamtuyenad
        // Login pass:admin4dm!n
        private readonly CakeshopContext db;
        private readonly IMapper _mapper;
        public AccountController(CakeshopContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Account", new { area = "Admin" });
            }
            else
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
                    if (nhanVien.MatKhau != model.Password.ToMd5Hash("4dm!n"))//"4dm!n" là saltkey.ToMd5Hash("4dm!n")
                    {
                        ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                    }
                    else
                    {
                        var phancong = db.PhanCongs.FirstOrDefault(x => x.MaNv == nhanVien.MaNv);
                        if (phancong==null)
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

                            return RedirectToAction("Index", "Account", new { area = "Admin" });
                            /*return RedirectToAction("/Admin/Account/Index");*/
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
            return Redirect("/Admin/Account/ThongBao");
        }
        #endregion

        #region Alert
        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult ThongBao()
        {
            return View();
        }
        #endregion
    }
}
