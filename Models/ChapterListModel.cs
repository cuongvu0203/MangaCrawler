using SayHenTai_WebApp.Infrastructure.Entities;

namespace SayHenTai_WebApp.Models
{
    public class ChapterListModel
    {
        public string? TenTruyen { get; set; }
        public string? TenKhac { get; set; }
        public string? TacGia { get; set; }
        public string? ViewCount { get; set; }
        public string? TheLoai { get; set; }
        public string? ImgCover { get; set; }
        public string? TrangThai { get; set; }
        public IReadOnlyList<CHUONG_MUC>? lstChuongMuc { get; set; }
    }
}
