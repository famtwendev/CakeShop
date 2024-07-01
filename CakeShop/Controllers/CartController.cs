using CakeShop.Data;
using CakeShop.ModelsView;
using CakeShop.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CakeShop.Services;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CakeShop.Controllers
{
    public class CartController : Controller
    {
        public readonly CakeshopContext db;
        private readonly IVnPayService _vnPayservice;
        public CartController(CakeshopContext context, IVnPayService vnPayservice)
        {
            db = context;
            _vnPayservice = vnPayservice;
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaHh = hangHoa.MaHh,
                    TenHH = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia,
                    Hinh = hangHoa.Hinh ?? string.Empty,
                    GiamGia = hangHoa.GiamGia,
                    SoLuong = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);


            return RedirectToAction("Index");
        }


        public IActionResult AddQuantity(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                item.SoLuong += 1;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }
        public IActionResult MinusQuantity(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                if (item.SoLuong > 1)
                    item.SoLuong -= 1;
                else if (item.SoLuong == 1)
                {
                    gioHang.Remove(item);
                }
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }


        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }


        [Authorize]
        [HttpGet]
        public IActionResult Checkout(int id)
        {
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }
            return View(Cart);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model, string payment)
        {
            if (ModelState.IsValid)
            {
                if (payment == "Thanh toán trực tuyến VNPay")
                {
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = Cart.Sum(p => p.ThanhTien),
                        CreatedDate = DateTime.Now,
                        Description = $"{model.HoTen} {model.DienThoai}",
                        FullName = model.HoTen,
                        OrderId = new Random().Next(1000, 100000)
                    };
                    return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
                }

                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                }
                var hoadon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    DienThoai = model.DienThoai ?? khachHang.DienThoai,
                    NgayDat = DateTime.Now,
                    NgayCan = DateTime.Now.AddDays(1),
                    NgayGiao = DateTime.Now.AddDays(5),
                    CachThanhToan = payment,
                    CachVanChuyen = "J&T Express",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu,
                };
                
                db.Database.BeginTransaction();
                try
                {
                    db.Add(hoadon);
                    db.SaveChanges();

                    var cthds = new List<ChiTietHd>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoadon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                            GiamGia = 0,// item.GiamGia,
                        });
                    }
                    db.AddRange(cthds);
                    db.SaveChanges();

                    db.Database.CommitTransaction();
                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                    // Gui Mail cho khach Hang
                    using (var client = new SmtpClient())
                    {
                        client.Connect("Smtp.gmail.com");
                        client.Authenticate("nptuyen121314@gmail.com", "ohmyxuononqjtzcl");
                        var bodyBuilder = new BodyBuilder();
                        bodyBuilder.HtmlBody = "<h3>HÓA ĐƠN MUA HÀNG:</h3></br>";
                        bodyBuilder.HtmlBody += $"<p>Mã Khách Hàng: {hoadon.MaKh}</p></br>";
                        bodyBuilder.HtmlBody += $"<p>Họ và tên: {hoadon.HoTen}</p></br>";
                        bodyBuilder.HtmlBody += $"<p>Địa chỉ nhận hàng: {hoadon.DiaChi}</p></br>";
                        bodyBuilder.HtmlBody += $"<p>Thông tin liên lạc: {hoadon.DienThoai}</p></br>";
                        bodyBuilder.HtmlBody += $"<p>Ngày đặt hàng: {hoadon.NgayDat.ToString("dd/MM/yyyy hh:mm tt")}</p></br>";
                        bodyBuilder.HtmlBody += $"<p>Ngày giao hàng: {hoadon.NgayGiao?.ToString("dd/MM/yyyy hh:mm tt")}</p></br>";
                        // Xây dựng nội dung email với từng chi tiết hóa đơn dưới dạng bảng HTML
                        bodyBuilder.HtmlBody += "<h3>Chi tiết hóa đơn:</h3></br>";
                        bodyBuilder.HtmlBody += "<table border='1'>";
                        bodyBuilder.HtmlBody += "<tr><th>Mã sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Giảm giá</th></tr>";
                        foreach (var cthd in cthds)
                        {
                            bodyBuilder.HtmlBody += "<tr>";
                            bodyBuilder.HtmlBody += $"<td>{cthd.MaHh}</td>";
                            bodyBuilder.HtmlBody += $"<td>{cthd.SoLuong}</td>";
                            bodyBuilder.HtmlBody += $"<td>{cthd.DonGia}</td>";
                            bodyBuilder.HtmlBody += $"<td>{cthd.GiamGia}</td>";
                            bodyBuilder.HtmlBody += "</tr>";
                        }
                        bodyBuilder.HtmlBody += "</table>";
                        var message = new MimeMessage
                        {
                            Body = bodyBuilder.ToMessageBody()
                        };
                        message.From.Add(new MailboxAddress("Cake Shop", "nptuyen121314@gmail.com"));
                        message.To.Add(new MailboxAddress("Hoa Don Mua Hang", khachHang.Email));
                        message.Subject = "Hóa Đơn mua hàng";
                        client.Send(message);

                        client.Disconnect(true);
                    }
                    TempData["Message"] = "Đặt hàng thành công";
                    return Redirect("/ThongBao");
                    /*return View("Success");*/
                }
                catch (Exception ex)
                {
                    db.Database.RollbackTransaction();
                }
            }
            return View(Cart);
        }
        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }


        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }


            // Lưu đơn hàng vô database

            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }

        [Authorize]
        public IActionResult HistoryBill(int? page)
        {
            var customerId = User.FindFirst("CustomerID")?.Value;
            var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
            int pageSize = 4;
            int pageNumber = page ?? 1;
            var cakeshopContext = db.HoaDons.Where(x => x.MaKh == khachHang.MaKh).ToPagedList(pageNumber, pageSize);
            return View(cakeshopContext);
        }
    }
}
