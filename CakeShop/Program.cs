using CakeShop.Data;
using CakeShop.Helpers;
using CakeShop.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Singleton
builder.Services.AddDbContext<CakeshopContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CakeShop"));
});

// Configure session state
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Configure AutoMapper https://docs.automapper.org/en/stable/Dependency-injection.html
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Configure authentication https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-8.0
/*builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
    options.LoginPath = "/KhachHang/DangNhap";
    options.AccessDeniedPath = "/AccessDenied";

    options.LoginPath = "/Admin/Login";
    options.AccessDeniedPath = "/AccessDenied";
});*/

// In Startup.cs or Program.cs (depending on the ASP.NET Core version)

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "CustomerCookie";
    options.DefaultChallengeScheme = "CustomerCookie";
})
.AddCookie("CustomerCookie", options =>
{
    options.LoginPath = "/KhachHang/DangNhap";
    options.AccessDeniedPath = "/AccessDenied";
})
.AddCookie("AdminCookie", options =>
{
    options.LoginPath = "/Admin/Login";
    options.AccessDeniedPath = "/AccessDenied";
});

builder.Services.AddSingleton<IVnPayService, VnPayService>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
