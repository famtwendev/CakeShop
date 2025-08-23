using System.Net;
using CakeShop.Data;
using CakeShop.Helpers;
using CakeShop.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Owl.reCAPTCHA;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// DB connection
var connectionString = builder.Configuration.GetValue<string>("CONNECTION_STRING");

// Nếu không có biến môi trường, sử dụng chuỗi kết nối mặc định từ appsettings.json (tuỳ chọn)
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("CakeShop");
}

// Configure AutoMapper https://docs.automapper.org/en/stable/Dependency-injection.html
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Sử dụng chuỗi kết nối
builder.Services.AddDbContext<CakeshopContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Configure session state
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Configure authentication https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-8.0
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "CustomerCookie";
    options.DefaultChallengeScheme = "CustomerCookie";
})
.AddCookie("CustomerCookie", options =>
{
    options.LoginPath = "/KhachHang/DangNhap";
    options.AccessDeniedPath = "/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(10);  // Thiết lập thời gian hết hạn là 10 ngày
    options.SlidingExpiration = true;  // Thiết lập Sliding Expiration nếu cần;
})

.AddCookie("AdminCookie", options =>
{
    options.LoginPath = "/Admin/Login";
    options.AccessDeniedPath = "/Admin/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(10);  // Thiết lập thời gian hết hạn là 10 ngày
    options.SlidingExpiration = true;  // Thiết lập Sliding Expiration nếu cần
});

// Configure VNPAY
builder.Services.AddScoped<IVnPayService, VnPayService>();
/*builder.Services.AddSingleton<IVnPayService, VnPayService>();*/
// reCAPTCHA
var siteKey = builder.Configuration["RECAPTCHA_SITEKEY"] ?? builder.Configuration["reCAPTCHA:SiteKey"];
var siteSecret = builder.Configuration["RECAPTCHA_SITESECRET"] ?? builder.Configuration["reCAPTCHA:SiteSecret"];

builder.Services.AddreCAPTCHAV2(x =>
{
    x.SiteKey = siteKey;
    x.SiteSecret = siteSecret;
});

builder.Services.Configure<ReCAPTCHASettings>(
    builder.Configuration.GetSection("reCAPTCHA"));


// Cấu hình để ứng dụng lắng nghe trên cổng 5124
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5125);  
});

// builder.WebHost.UseUrls("http://0.0.0.0:5125");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");
    // Đặt route này phía dưới để nó không ảnh hưởng đến các route khác
/*    endpoints.MapControllerRoute(
        name: "admin_default",
        pattern: "Admin",
        defaults: new { area = "Admin", controller = "Admin", action = "Index" }
    );
*/

});
#pragma warning restore ASP0014 // Suggest using top level route registrations
app.Run();
