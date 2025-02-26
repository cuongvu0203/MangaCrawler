using MongoDB.Bson;
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
                return await _TruyenData.MgGetAllDanhSachTruyen();
            }, CoreConstants.DefaultCacheTimeDay);
        }

        public async Task<ChapterListModel> getChuongMucTruyenByIdTruyen(string id)
        {
            ChapterListModel model = new ChapterListModel();
            var cacheKeyTruyen = CreateCacheKey.GetTruyenByIdAsync(id);
            var dbTruyen = await _memoryCacheService.GetOrCreate(cacheKeyTruyen, async () =>
            {
                return await _TruyenData.MgGetTruyenByIdAsync(id);
            }, CoreConstants.DefaultCacheTimeDay);

            var cacheKey = CreateCacheKey.BuildDanhSachChuongMucByIdTruyenAsync(id);
            var dbChuongMuc = await _memoryCacheService.GetOrCreate(cacheKey, async () =>
            {
                return await _TruyenData.MgGetChuongMucTruyenByIdTruyen(id);
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

        public async Task<ChapterDetail> getDetailTruyenByIdTruyenAndIdChuongMuc(string IdChuongMuc, string IdTruyen)
        {
            ChapterDetail model = new ChapterDetail();
            var cacheKeyTruyen = CreateCacheKey.GetTruyenByIdAsync(IdTruyen);
            var dbTruyen = await _memoryCacheService.GetOrCreate(cacheKeyTruyen, async () =>
            {
                return await _TruyenData.MgGetTruyenByIdAsync(IdTruyen);
            }, CoreConstants.DefaultCacheTimeDay);

            var cacheKey = CreateCacheKey.GetDetailTruyenByIdTruyenAndIdChuongMucAsync(IdChuongMuc, IdTruyen);
            var lstTruyenChiTiet = await _memoryCacheService.GetOrCreate(cacheKey, async () =>
            {
                return await _TruyenData.MgGetDetailTruyenByIdTruyenAndIdChuongMucAsync(IdChuongMuc, IdTruyen);
            }, CoreConstants.DefaultCacheTimeDay);


            // Lấy danh sách chương từ cache hoặc database
            var cacheKeyChuongMuc = CreateCacheKey.BuildDanhSachChuongMucByIdTruyenAsync(IdTruyen);
            var lstChuongMuc = await _memoryCacheService.GetOrCreate(cacheKeyChuongMuc, async () =>
            {
                return await _TruyenData.MgGetChuongMucTruyenByIdTruyen(IdTruyen);
            }, CoreConstants.DefaultCacheTimeDay);

            // Chuyển về List để sử dụng FindIndex
            var chuongMucList = lstChuongMuc.ToList();

            // Chuyển `IdChuongMuc` thành ObjectId để so sánh
            var currentObjectId = new ObjectId(IdChuongMuc);

            // Sắp xếp danh sách chương theo _id (thứ tự tạo trong MongoDB)
            chuongMucList = chuongMucList.OrderBy(x => new ObjectId(x.Id)).ToList();

            // Tìm vị trí chương hiện tại
            var currentIndex = chuongMucList.FindIndex(x => x.Id == IdChuongMuc);

            // Lấy chương trước và sau nếu có
            var prevChapter = currentIndex > 0 ? chuongMucList[currentIndex - 1] : null;
            var nextChapter = currentIndex < chuongMucList.Count - 1 ? chuongMucList[currentIndex + 1] : null;

            // Tạo URL chương hiện tại, trước và sau
            var currenUrl = $"/chi-tiet-truyen/{CoreUtility.UrlFromUnicode(dbTruyen.TEN)}-chapter-{IdChuongMuc}-{IdTruyen}";
            var PrevUrl = prevChapter != null
                ? $"/chi-tiet-truyen/{CoreUtility.UrlFromUnicode(dbTruyen.TEN)}-chapter-{prevChapter.Id}-{IdTruyen}"
                : null;
            var NextUrl = nextChapter != null
                ? $"/chi-tiet-truyen/{CoreUtility.UrlFromUnicode(dbTruyen.TEN)}-chapter-{nextChapter.Id}-{IdTruyen}"
                : null;


            model.TenTruyen = dbTruyen.TEN;
            model.ChapterName = lstChuongMuc?.FirstOrDefault(x => x.Id == IdChuongMuc)?.TEN;
            model.UrlCurrent = currenUrl;
            model.UrlPrevious = PrevUrl;
            model.UrlNext = NextUrl;
            model.lstChapter = lstTruyenChiTiet;
            return model;
        }
    }
}
