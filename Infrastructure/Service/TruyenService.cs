using SayHenTai_WebApp.Infrastructure.Caching;
using SayHenTai_WebApp.Infrastructure.Data;
using SayHenTai_WebApp.Infrastructure.Entities;
using SayHenTai_WebApp.Models;
using System.Data.SqlClient;

namespace SayHenTai_WebApp.Infrastructure.Service
{
    public class TruyenService
    {
        private readonly MemoryCacheService _memoryCacheService;
        private readonly TruyenData _TruyenData;
        public TruyenService(
           MemoryCacheService memoryCacheService,
           TruyenData TruyenData)
        {
            _memoryCacheService = memoryCacheService;
            _TruyenData = TruyenData;
        }

        public async Task<IReadOnlyList<TRUYEN>> GetAllDanhSachTruyen()
        {
            var cacheKey = CreateCacheKey.BuildDanhSachTruyenAsync();
            return await _memoryCacheService.GetOrCreate(cacheKey, async () =>
            {
                return await _TruyenData.GetAllDanhSachTruyen();
            }, CoreConstants.DefaultCacheTimeDay);
        }

        public async Task<ChapterListModel> getChuongMucTruyenByIdTruyen(long id)
        {
            ChapterListModel model = new ChapterListModel();
            var cacheKeyTruyen = CreateCacheKey.GetTruyenByIdAsync(id);
            var dbTruyen = await _memoryCacheService.GetOrCreate(cacheKeyTruyen, async () =>
            {
                return await _TruyenData.GetTruyenByIdAsync(id);
            }, CoreConstants.DefaultCacheTimeDay);

            var cacheKey = CreateCacheKey.BuildDanhSachChuongMucByIdTruyenAsync(id);
            var dbChuongMuc = await _memoryCacheService.GetOrCreate(cacheKey, async () =>
            {
                return await _TruyenData.GetChuongMucTruyenByIdTruyen(id);
            }, CoreConstants.DefaultCacheTimeDay);

            model.TenTruyen = dbTruyen.TEN;
            model.TenKhac = "";
            model.TacGia = dbTruyen.TAC_GIA;
            model.ImgCover = dbTruyen.IMAGE_COVER;
            model.ViewCount = dbTruyen.SO_LAN_DOC.ToString();
            model.TheLoai = dbTruyen.THE_LOAI;
            model.TrangThai = dbTruyen.TRANG_THAI;
            model.lstChuongMuc = dbChuongMuc;
            return model;
        }

        public async Task<ChapterDetail> getDetailTruyenByIdTruyenAndIdChuongMuc(long IdChuongMuc, long IdTruyen)
        {
            ChapterDetail model = new ChapterDetail();
            var cacheKeyTruyen = CreateCacheKey.GetTruyenByIdAsync(IdTruyen);
            var dbTruyen = await _memoryCacheService.GetOrCreate(cacheKeyTruyen, async () =>
            {
                return await _TruyenData.GetTruyenByIdAsync(IdTruyen);
            }, CoreConstants.DefaultCacheTimeDay);

            var cacheKey = CreateCacheKey.GetDetailTruyenByIdTruyenAndIdChuongMucAsync(IdChuongMuc, IdTruyen);
            var lstTruyenChiTiet = await _memoryCacheService.GetOrCreate(cacheKey, async () =>
            {
                return await _TruyenData.GetDetailTruyenByIdTruyenAndIdChuongMucAsync(IdChuongMuc, IdTruyen);
            }, CoreConstants.DefaultCacheTimeDay);


            var cacheKeyChuongMuc = CreateCacheKey.BuildDanhSachChuongMucByIdTruyenAsync(IdTruyen);
            var lstChuongMuc = await _memoryCacheService.GetOrCreate(cacheKeyChuongMuc, async () =>
            {
                return await _TruyenData.GetChuongMucTruyenByIdTruyen(IdTruyen);
            }, CoreConstants.DefaultCacheTimeDay);

            var currenUrl = string.Format("/chi-tiet-truyen/{0}-chapter-{1}-{2}", CoreUtility.UrlFromUnicode(dbTruyen.TEN), IdChuongMuc, IdTruyen);
            var PrevUrl = string.Format("/chi-tiet-truyen/{0}-chapter-{1}-{2}", CoreUtility.UrlFromUnicode(dbTruyen.TEN), (IdChuongMuc + 1), IdTruyen);
            var NextUrl = string.Format("/chi-tiet-truyen/{0}-chapter-{1}-{2}", CoreUtility.UrlFromUnicode(dbTruyen.TEN), (IdChuongMuc - 1), IdTruyen);

            model.TenTruyen = dbTruyen.TEN;
            model.ChapterName = lstChuongMuc?.FirstOrDefault(x => x.ID == IdChuongMuc)?.TEN;
            model.UrlCurrent = currenUrl;
            model.UrlPrevious = PrevUrl;
            model.UrlNext = NextUrl;
            model.lstChapter = lstTruyenChiTiet;
            return model;
        }
    }
}
