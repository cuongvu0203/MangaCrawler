using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SayHenTai_WebApp.Infrastructure.Caching;
using SayHenTai_WebApp.Infrastructure.Service;
using SayHenTai_WebApp.Models;
using System.Text;

namespace SayHenTai_WebApp.Controllers
{
    public class LeechController : Controller
    {
        private readonly LeechService _leechService;
        private readonly MemoryCacheService _memoryCacheService;
        private readonly IDataProtector _dataProtector;
        public LeechController(
            MemoryCacheService memoryCacheService,
            LeechService leechService,
            IDataProtectionProvider dataProtectionProvider)
        {
            _memoryCacheService = memoryCacheService;
            _dataProtector = dataProtectionProvider.CreateProtector("account");
            _leechService = leechService;

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeechLinkSayHenTai(LeechModel model)
        {
            var Result = await _leechService.LeechTruyenByUrl(model.url);

            ViewBag.CheckStatus = Result;
            return View("Index",model);
        }
    }

}
