using AutoMapper;
using CakeShop.Data;
using CakeShop.Helpers;
using CakeShop.ModelsView;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;

namespace CakeShop.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly CakeshopContext db;
        private readonly IMapper _mapper;
        public KhachHangController(CakeshopContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        #region Register

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(RegisterVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtil.GenerateRamdomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true; // xử lý khi dùng Mail để active
                    khachHang.VaiTro = 0;

                    if (Hinh != null && Hinh.Length > 0)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    db.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Index", "HangHoa");
                }
                catch (Exception ex)
                {
                    var mess = $"{ex.Message} shh";
                }
            }
            return View();
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            /*if (User.Identity.IsAuthenticated) //Coi dang nhap chua
            {
                return RedirectToAction("Profile", "Customer");
            }*/
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
                if (khachHang == null)
                {
                    ModelState.AddModelError("loi", "Không có khách hàng này");
                }
                else
                {
                    if (!khachHang.HieuLuc)
                    {
                        ModelState.AddModelError("loi", "Tài khoản đã bị khóa. Vui lòng liên hệ Admin.");
                    }
                    else
                    {
                        if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
                        {
                            ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                        }
                        else
                        {
                            var claims = new List<Claim> {
                                new Claim(ClaimTypes.Email, khachHang.Email),
                                new Claim(ClaimTypes.Name, khachHang.HoTen),
                                new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.MaKh),

								//claim - role động
								new Claim(ClaimTypes.Role, "Customer")
                            };

                            /*                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                                                        await HttpContext.SignInAsync(claimsPrincipal);*/

                            var claimsIdentity = new ClaimsIdentity(claims, "CustomerCookie");
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync("CustomerCookie", claimsPrincipal);

                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                    }
                }
            }
            return View();
        }
        #endregion

        [Authorize(AuthenticationSchemes = "CustomerCookie")]
        public IActionResult Profile()
        {
            var customerId = User.FindFirst("CustomerID")?.Value;

            var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);

            if (khachHang == null)
            {
                return NotFound();
            }

            var viewModel = new ThayDoiThongTinVM
            {
                MaKh = khachHang.MaKh,
                HoTen = khachHang.HoTen,
                GioiTinh = khachHang.GioiTinh,
                NgaySinh = khachHang.NgaySinh,
                DiaChi = khachHang.DiaChi ?? "",
                DienThoai = khachHang.DienThoai,
                Email = khachHang.Email,
                Hinh = khachHang.Hinh
            };

            return View(viewModel);
        }

        [Authorize(AuthenticationSchemes = "CustomerCookie")]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            TempData["Message"] = "Quý khách đã đăng xuất tài khoản!";
            return Redirect("/ThongBao");
        }

        #region ThayDoiThongin
        [HttpPost]
        [Authorize(AuthenticationSchemes = "CustomerCookie")]
        public IActionResult ThayDoiThongTin(ThayDoiThongTinVM model, IFormFile Hinh)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    // Lấy thông tin khách hàng từ database
                    var customerId = User.FindFirst("CustomerID")?.Value; // Lấy Id của khách hàng từ User Claims

                    var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);

                    if (khachHang == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin mới từ form vào đối tượng khách hàng
                    khachHang.HoTen = model.HoTen;
                    khachHang.GioiTinh = model.GioiTinh;
                    khachHang.NgaySinh = model.NgaySinh;
                    khachHang.DiaChi = model.DiaChi;
                    khachHang.DienThoai = model.DienThoai;
                    khachHang.Email = model.Email;

                    if (Hinh != null && Hinh.Length > 0)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.Update(khachHang);
                    db.SaveChanges();

                    // Chuyển hướng về trang profile hoặc trang chủ
                    /*return RedirectToAction("Profile", "KhachHangController");*/ // Thay "TenController" bằng tên Controller thực tế
                    TempData["Message"] = "Thông tin đã được cập nhật thành công";
                    return Redirect("/ThongBao");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu cần thiết
                    var mess = $"{ex.Message} shh";
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật thông tin.");
                }
            }

            // Nếu ModelState không hợp lệ thì hiển thị lại form với thông báo lỗi
            return Redirect("/");
        }
        #endregion

    }

}
