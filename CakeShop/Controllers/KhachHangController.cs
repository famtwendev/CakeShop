using AutoMapper;
using CakeShop.Data;
using CakeShop.Helpers;
using CakeShop.ModelsView;
using CakeShop.ModelsView.ForgotPassword;
using CakeShop.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using Owl.reCAPTCHA.v2;
using System.Security.Claims;

namespace CakeShop.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly CakeshopContext db;
        private readonly IMapper _mapper;
        private readonly IreCAPTCHASiteVerifyV2 _siteVerify;
        public KhachHangController(CakeshopContext context, IMapper mapper, IreCAPTCHASiteVerifyV2 siteVerify)
        {
            db = context;
            _mapper = mapper;
            _siteVerify = siteVerify;
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
                    var existingKhachHang = db.KhachHangs.FirstOrDefault(kh => kh.Email == model.Email);
                    if (existingKhachHang != null)
                    {
                        ModelState.AddModelError("Email", "Email đã được sử dụng bởi một tài khoản khác.");
                        return View(model); // Return the view with validation errors
                    }
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtil.GenerateRamdomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true; // xử lý khi dùng Mail để active
                    khachHang.VaiTro = 0;

                    if (Hinh != null && Hinh.Length > 0)
                    {
                        // Danh sách các đuôi tệp hợp lệ
                        var permittedExtensions = new[] { ".png", ".jpg", ".jpeg" };

                        // Lấy đuôi tệp
                        var ext = Path.GetExtension(Hinh.FileName).ToLowerInvariant();

                        // Kiểm tra đuôi tệp
                        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                        {
                            ModelState.AddModelError("Hinh", "Chỉ cho phép tệp hình ảnh có đuôi .png, .jpg, .jpeg.");
                            return View(model);
                        }

                        // Kiểm tra kiểu MIME
                        var mimeType = Hinh.ContentType.ToLower();
                        if (!mimeType.StartsWith("image/"))
                        {
                            ModelState.AddModelError("Hinh", "Tệp không phải là hình ảnh.");
                            return View(model);
                        }

                        // Nếu tệp hợp lệ, lưu tệp
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    db.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Index", "HangHoa");
                }
                catch (Exception ex)
                {
                    var mess = $"{ex.Message} shh";
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra trong quá trình xử lý. Vui lòng thử lại.");
                }
            }
            return View();
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (string.IsNullOrEmpty(model.Captcha))
            {
                ModelState.AddModelError("loi", "Không nhập Captcha.");
                return View(model);
            }
            var response = await _siteVerify.Verify(new Owl.reCAPTCHA.reCAPTCHASiteVerifyRequest
            {
                Response = model.Captcha,
                RemoteIp = HttpContext.Connection.RemoteIpAddress.ToString(),
            });

            if(!response.Success)
            {
                ModelState.AddModelError("loi", "Captcha không đúng!.");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                /*             var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);*/
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName || kh.Email == model.UserName);
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
                            await HttpContext.SignOutAsync("AdminCookie");
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
            return View(model);
        }
        #endregion

        #region Profile
        [Authorize]
        public async Task<IActionResult> Profile()
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
        #endregion

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            TempData["Message"] = "Quý khách đã đăng xuất tài khoản!";
            return Redirect("/ThongBao");
        }

        #region ThayDoiThongin
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ThayDoiThongTin(ThayDoiThongTinVM model, IFormFile? Hinh)
        {
            if (ModelState.IsValid)
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

                    // Kiểm tra và xử lý tệp hình ảnh nếu có
                    if (Hinh != null && Hinh.Length > 0)
                    {
                        // Danh sách các đuôi tệp hợp lệ
                        var permittedExtensions = new[] { ".png", ".jpg", ".jpeg" };

                        // Lấy đuôi tệp của Hinh
                        var ext = Path.GetExtension(Hinh.FileName).ToLowerInvariant();

                        // Kiểm tra đuôi tệp
                        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                        {
                            ModelState.AddModelError("Hinh", "Chỉ cho phép tệp hình ảnh có đuôi .png, .jpg, .jpeg.");
                            return RedirectToAction(nameof(Profile)); // Trả về view với thông báo lỗi
                        }

                        // Lưu tệp hình vào thư mục và lấy tên tệp đã lưu
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.Update(khachHang);
                    await db.SaveChangesAsync();

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
            return RedirectToAction(nameof(Profile));
        }
        #endregion


        #region QuenMatKHau
        [HttpGet]
        public async Task<IActionResult> QuenMatKhau()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> QuenMatKhau(QuenMatKhau model)
        {
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.Email == model.Email);
                if (khachHang == null)
                {
                    ModelState.AddModelError("loi", "Email không tồn tại !");
                    return View(model);
                }
                khachHang.RandomKey = MyUtil.getSoNgauNhien().ToString();
                // Đánh dấu là đã thay đổi
                db.Entry(khachHang).State = EntityState.Modified;
                await db.SaveChangesAsync();
                string htmlBody;
                htmlBody = "<h3>QUÝ KHÁCH VUI LÒNG XÁC NHẬN TÀI KHOẢN:</h3></br>";
                htmlBody += $"<b><h4>Mã OTP là: {khachHang.RandomKey} </h4></b></br>";
                htmlBody += "<p>Quý khách vui lòng không phản hồi lại email này! Trân trọng kính chào!</P>";
                GmailService.sendGmail("nptuyen121314@gmail.com", "CAKE SHOP", khachHang.Email, "XÁC THỰC ĐỔI MẬT KHẨU", htmlBody);
                /*return RedirectToAction("XacThucOTP", "KhachHang", new { email = model.Email });*/
                TempData["Email"] = model.Email;
                return RedirectToAction("XacThucOTP", "KhachHang");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult XacThucOTP()
        {
            if (TempData["Email"] != null)
            {
                ViewBag.Email = TempData["Email"].ToString();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> XacThucOTP(AuthOTP model)
        {
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.Email == model.email && kh.RandomKey == model.Total);
                if (khachHang == null)
                {
                    ModelState.AddModelError("loi", "Mã OTP không hợp lệ hoặc đã hết hạn!");
                    ViewBag.Email = model.email;
                    return View(model);
                }
                string matkhau = MyUtil.GeneratePassword();

                khachHang.MatKhau = matkhau.ToMd5Hash(khachHang.RandomKey);
                khachHang.HieuLuc = true; // xử lý khi dùng Mail để active
                khachHang.VaiTro = 0;
                db.Entry(khachHang).State = EntityState.Modified;
                await db.SaveChangesAsync();
                string htmlBody;
                htmlBody = "<h3>CẤP MẬT KHẨU MỚI CHO TÀI KHOẢN:</h3></br>";
                htmlBody += $"<b><h4>Mật khẩu mới là: {matkhau} </h4></b></br>";
                htmlBody += $"<b><p>Sau khi đăng nhập bằng mật khẩu mới. Vui lòng đổi lại mật khẩu phù hợp với cá nhân!</p></b></br>";
                htmlBody += "<p>Quý khách vui lòng không phản hồi lại email này! Trân trọng kính chào!</P>";
                GmailService.sendGmail("nptuyen121314@gmail.com", "CAKE SHOP", khachHang.Email, "CẤP MẬT KHẨU CHO TÀI KHOẢN", htmlBody);
                TempData["Message"] = "Chúng tôi đã gửi mật khẩu mới. Vui lòng kiểm tra email của bạn.";
                return Redirect("/ThongBao");
            }
            ViewBag.Email = model.email;
            return View(model);
        }
        #endregion


        #region DoiMatKhau
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DoiMatKhau()
        {
            return View();
        }

        [HttpPost, ActionName("DoiMatKhau")]
        [Authorize]
        public async Task<IActionResult> DoiMatKhauConfirm(DoiMatKhau model)
        {
            if (ModelState.IsValid)
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
                    if (model.matKhauHienTai.ToMd5Hash(khachHang.RandomKey) != khachHang.MatKhau)
                    {
                        ModelState.AddModelError("loi", "Mật khẩu hiện tại không đúng");
                        return View(model);
                    }
                    if(model.matKhauMoi != model.matKhauMoiNhapLai)
                    {
                        ModelState.AddModelError("loi", "Mật khẩu mới nhập lại không khớp");
                        return View(model);
                    }
                    khachHang.RandomKey = MyUtil.GenerateRamdomKey();
                    khachHang.MatKhau = model.matKhauMoi.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true; // xử lý khi dùng Mail để active
                    khachHang.VaiTro = 0;
                    db.Entry(khachHang).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["Message"] = "Quý khách đã đổi mật khẩu thành công!";
                    return Redirect("/ThongBao");
                }
                catch (Exception ex)
                {
                    var mess = $"{ex.Message} shh";
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật thông tin.");
                }
            }
            return View(model);
        }
        #endregion
    }
}
