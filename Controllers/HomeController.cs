using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using SayHenTai_WebApp.Infrastructure.Caching;
using SayHenTai_WebApp.Infrastructure.Entities;
using SayHenTai_WebApp.Infrastructure.Service;
using SayHenTai_WebApp.Models;
using System.Diagnostics;

namespace SayHenTai_WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TruyenService _truyenService;
        private readonly MemoryCacheService _memoryCacheService;
        private readonly IDataProtector _dataProtector;
        public HomeController(ILogger<HomeController> logger,
            MemoryCacheService memoryCacheService,
            TruyenService truyenService,
            IDataProtectionProvider dataProtectionProvider)
        {
            _logger = logger;
            _memoryCacheService = memoryCacheService;
            _dataProtector = dataProtectionProvider.CreateProtector("account");
            _truyenService = truyenService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _truyenService.GetAllDanhSachTruyen();
            return View(model);
        }

        public async Task<IActionResult> ChuongMucDetails(long id)
        {
            var model = await _truyenService.getChuongMucTruyenByIdTruyen(id);
            return View(model);
        }

        public async Task<IActionResult> Details(long IdChuongMuc, long IdTruyen)
        {
            var model = await _truyenService.getDetailTruyenByIdTruyenAndIdChuongMuc(IdChuongMuc, IdTruyen);
            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}