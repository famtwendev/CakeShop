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
/*    [Authorize(Roles = SD.Role_Admin)]*/
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

        [Authorize(Roles = SD.Role_Admin, AuthenticationSchemes = "AdminCookie")]
        public IActionResult Index()
        {
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
                        {
                            var claims = new List<Claim> {
                            new Claim(ClaimTypes.Name, nhanVien.HoTen),
                            new Claim(ClaimTypes.NameIdentifier, nhanVien.MaNv),
                            new Claim(ClaimTypes.Role, SD.Role_Admin),

                            };
                            // Thêm claim động dựa trên phòng ban  //claim - role động
                            if (phancong.MaPb == SD.RolePB_BGD)
                            {
                                claims.Add(new Claim(SD.RolePB_BGD, phancong.MaPb));
                            }
                            else if (phancong.MaPb == SD.RolePB_PKT)
                            {
                                claims.Add(new Claim(SD.RolePB_PKT, phancong.MaPb));
                            }
                            else if (phancong.MaPb == SD.RolePB_PKTo)
                            {
                                claims.Add(new Claim(SD.RolePB_PKTo, phancong.MaPb));
                            }
                            else if (phancong.MaPb == SD.RolePB_PNS)
                            {
                                claims.Add(new Claim(SD.RolePB_PNS, phancong.MaPb));
                            }

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(claimsPrincipal);

                            return RedirectToAction("Index", "Admin");

                        }
                    }
                }
            }

            /*TempData["Message"] = "Không thể đăng nhập. Vui lòng liên hệ Admin!";
            return Redirect("/404");model*/
            return View();
        }
        #endregion

        #region Logout
        [Authorize(Roles = SD.Role_Admin, AuthenticationSchemes = "AdminCookie")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            TempData["Message"] = "Đăng xuất thành công!";
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
