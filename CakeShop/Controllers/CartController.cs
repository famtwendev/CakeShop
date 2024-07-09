using CakeShop.Data;
using CakeShop.ModelsView;
using CakeShop.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CakeShop.Services;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
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
            if (quantity <= 0)
            {
                quantity = 1;
            }
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
            if (item != null && item.SoLuong >= 0 && item.SoLuong < 99)
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
            if (item != null && item.SoLuong >= 0 && item.SoLuong < 99)
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
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                /*if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                }*/
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
                    GhiChu = model.GhiChu ?? "",
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
                            GiamGia = item.GiamGia,// item.GiamGia,
                        });
                    }
                    db.AddRange(cthds);
                    db.SaveChanges();

                    db.Database.CommitTransaction();
                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                    // Soạn Mail cho khach Hang
                    string htmlBody;
                    htmlBody = "<h3>HÓA ĐƠN MUA HÀNG:</h3></br>";
                    htmlBody += $"<p>Mã Khách Hàng: {hoadon.MaKh}</p></br>";
                    htmlBody += $"<p>Họ và tên: {hoadon.HoTen}</p></br>";
                    htmlBody += $"<p>Địa chỉ nhận hàng: {hoadon.DiaChi}</p></br>";
                    htmlBody += $"<p>Thông tin liên lạc: {hoadon.DienThoai}</p></br>";
                    htmlBody += $"<p>Phương thức thanh toán: {payment}</p></br>";
                    htmlBody += $"<p>Ngày đặt hàng: {hoadon.NgayDat.ToString("dd/MM/yyyy hh:mm tt")}</p></br>";
                    htmlBody += $"<p>Ngày giao hàng: {hoadon.NgayGiao?.ToString("dd/MM/yyyy hh:mm tt")}</p></br>";
                    // Xây dựng nội dung email với từng chi tiết hóa đơn dưới dạng bảng HTML
                    htmlBody += "<h3>Chi tiết hóa đơn:</h3></br>";
                    htmlBody += "<table border='1'>";
                    htmlBody += "<tr><th>Mã sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Giảm giá</th></tr>";
                    foreach (var cthd in cthds)
                    {
                        htmlBody += "<tr>";
                        htmlBody += $"<td>{cthd.MaHh}</td>";
                        htmlBody += $"<td>{cthd.SoLuong}</td>";
                        htmlBody += $"<td>{cthd.DonGia}</td>";
                        htmlBody += $"<td>{cthd.GiamGia}</td>";
                        htmlBody += "</tr>";
                    }
                    htmlBody += "</table>";
                    GmailService.sendGmail("nptuyen121314@gmail.com", "ESSENCE SHOP", khachHang.Email, "Hóa Đơn mua hàng", htmlBody);
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
