Scaffolding has generated all the files and added the required dependencies.

However the Application's Startup code may require additional changes for things to work end to end.
Add the following code to the Configure method in your Application's Startup class if not already done:

        app.UseEndpoints(endpoints =>
        {
          endpoints.MapControllerRoute(
            name : "areas",
            pattern : "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );
        });


       // Chức năng
        $(document).ready(function () {
            // Thiết lập thanh trượt cho khoảng giá
            $(".slider-range-price").slider({
                range: true,
                min: 1000,
                max: 3000000,
                values: [1000, 3000000],
                slide: function (event, ui) {
                    $(".range-price").text("Range: $" + ui.values[0] + " - $" + ui.values[1]); // Cập nhật văn bản hiển thị
                },
                stop: function (event, ui) {
                    applyFilters(); // Áp dụng bộ lọc khi ngừng kéo thả
                }
            });

            // Lắng nghe sự kiện thay đổi trong dropdown menu
            $("#sortByselect").change(function () {
                applyFilters(); // Áp dụng bộ lọc khi thay đổi lựa chọn
            });
            // Hàm áp dụng bộ lọc
            function applyFilters() {
                var minPrice = $(".slider-range-price").slider("values", 0); // Lấy giá trị min của thanh trượt
                var maxPrice = $(".slider-range-price").slider("values", 1); // Lấy giá trị max của thanh trượt
                var sort = $("#sortByselect").val(); // Lấy giá trị của dropdown menu

                var url = "/HangHoa/Index?"; // Đường dẫn đến action YeuCau

                // Thêm tham số min và max nếu có
                if (minPrice !== 1000 || maxPrice !== 3000000) {
                    url += "min=" + minPrice + "&max=" + maxPrice + "&";
                }
                // Thêm tham số sort nếu có
                if (sort) {
                    url += "sort=" + sort + "&";
                }
                url = url.slice(0, -1); // Loại bỏ dấu & cuối cùng nếu có
                window.location.href = url; // Chuyển hướng đến URL đã xây dựng
            }
        });

                public IActionResult Index(int? loai, int? page, string? sort, int? min, int? max)
        {

            var hangHoas = db.HangHoas.AsQueryable();
            int pageSize = 6;
            int pageNumber = page ?? 1;
            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }
            if (min.HasValue && max.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.DonGia >= min && p.DonGia <= max);
            }

            switch (sort)
            {
                case "increase":
                    hangHoas = hangHoas.OrderBy(p => p.DonGia);
                    break;
                case "decrease":
                    hangHoas = hangHoas.OrderByDescending(p => p.DonGia);
                    break;
                default:
                    hangHoas = hangHoas.OrderByDescending(p => p.DonGia); // Mặc định sắp xếp theo giảm dần
                    break;
            }
          /*  int pageSize = 6;
            int pageNumber = page ?? 1;*/

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai,
                Giamgia = p.GiamGia,
            }).ToPagedList(pageNumber, pageSize);

            return View(result);
            /*            var hangHoas = db.HangHoas.AsQueryable();
                        if (loai.HasValue)
                        {
                            hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
                        }
                        var result = hangHoas.Select(p => new HangHoaVM
                        {
                            MaHh = p.MaHh,
                            TenHH = p.TenHh,
                            DonGia = p.DonGia,
                            Hinh = p.Hinh ?? "",
                            MoTaNgan = p.MoTaDonVi ?? "",
                            TenLoai = p.MaLoaiNavigation.TenLoai,
                            Giamgia = p.GiamGia,
                        });
                        return View(result);*/
        }
Customer
https://cdnjs.com/libraries/font-awesome
https://cdnjs.com/libraries/jqueryui/1.12.1
https://cdnjs.com/libraries/OwlCarousel2/2.2.1
https://cdnjs.com/libraries/jquery/2.2.4
https://cdnjs.com/libraries/font-awesome/4.7.0