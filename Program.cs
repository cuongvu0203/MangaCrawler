using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.WebEncoders;
using SayHenTai_WebApp.Infrastructure;
using SayHenTai_WebApp.Infrastructure.Caching;
using SayHenTai_WebApp.Infrastructure.Data;
using SayHenTai_WebApp.Infrastructure.Service;
using System.Configuration;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the configuration
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.Configure<Connections>(configuration.GetSection("Connections"));

builder.Services.AddDataProtection();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "SayHenTaiWebApp";
    options.Cookie.HttpOnly = true;
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/accessdenied.html";
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
});
builder.Services.AddSingleton<MemoryCacheService>();
builder.Services.AddSingleton<LeechService>();
builder.Services.AddSingleton<LeechTruyenData>();
builder.Services.AddSingleton<TruyenService>();
builder.Services.AddSingleton<TruyenData>();
builder.Services.AddResponseCompression(options => options.Providers.Add<GzipCompressionProvider>());
builder.Services.Configure<WebEncoderOptions>(webEncoderOptions => webEncoderOptions.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));
builder.Services.AddControllersWithViews();

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

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute("leech-truyen", "leech-truyen", new { controller = "Leech", action = "Index" });
app.MapControllerRoute("chuong-muc-truyen-details", "chuong-muc-truyen/{title}-{id}", new { controller = "Home", action = "ChuongMucDetails" });
app.MapControllerRoute("chi-tiet-truyen-details", "chi-tiet-truyen/{title}-chapter-{IdChuongMuc}-{IdTruyen}", new { controller = "Home", action = "Details" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
