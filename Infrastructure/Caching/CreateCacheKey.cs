using SayHenTai_WebApp.Infrastructure.Entities;

namespace SayHenTai_WebApp.Infrastructure.Caching
{
    public static class CreateCacheKey
    {
        public static string BuildDanhSachChuongMucByIdTruyenAsync(decimal IdTruyen) => $"BuildDanhSachChuongMucByIdTruyenAsync-m{IdTruyen}";

        public static string BuildDanhSachTruyenAsync() => $"BuildDanhSachTruyenAsync";
        public static string GetTruyenByIdAsync(long id) => $"GetTruyenByIdAsync-m{id}";

        public static string GetDetailTruyenByIdTruyenAndIdChuongMucAsync(long idChuongMuc, long idTruyen) => $"GetDetailTruyenByIdTruyenAndIdChuongMucAsync-cm{idChuongMuc}-t{idTruyen}";
    }
}
