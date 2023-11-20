using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using WebBanThu.Helpers;
using WebBanThu.Interface;
using WebBanThu.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.LoginPath = "/User/login";
    option.AccessDeniedPath ="/User/login";
    //option.ReturnUrlParameter = "returnUrl";
    //option.AccessDeniedPath = "/User/login";
    //    ////option.SlidingExpiration = true;
    //    //option.ExpireTimeSpan = TimeSpan.FromMinutes(60 * 1);
    //    ////option.AccessDeniedPath = "/User/Forbidden";

});
// Đọc cấu hình từ appsettings.json
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();


// Thêm dịch vụ cấu hình

// Đăng ký dịch vụ SMTP cấu hình


// Rest of the configuration code...
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
      name: "Myarea",
      pattern: "{area:exists}/{controller=AdminHome}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
