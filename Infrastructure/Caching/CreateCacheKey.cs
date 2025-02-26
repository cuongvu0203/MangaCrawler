using SayHenTai_WebApp.Infrastructure.Entities;

namespace SayHenTai_WebApp.Infrastructure.Caching
{
    public static class CreateCacheKey
    {
        public static string BuildDanhSachChuongMucByIdTruyenAsync(string IdTruyen) => $"BuildDanhSachChuongMucByIdTruyenAsync-m{IdTruyen}";

        public static string BuildDanhSachTruyenAsync() => $"BuildDanhSachTruyenAsync";
        public static string GetTruyenByIdAsync(string id) => $"GetTruyenByIdAsync-m{id}";

        public static string GetDetailTruyenByIdTruyenAndIdChuongMucAsync(string idChuongMuc, string idTruyen) => $"GetDetailTruyenByIdTruyenAndIdChuongMucAsync-cm{idChuongMuc}-t{idTruyen}";
    }
}
