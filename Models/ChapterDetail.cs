using SayHenTai_WebApp.Infrastructure.Entities;

namespace SayHenTai_WebApp.Models
{
    public class ChapterDetail
    {
        public string? ChapterName { get; set; }
        public string? TenTruyen { get; set; }
        public string? UrlPrevious { get; set; }
        public string? UrlNext { get; set; }
        public string? UrlCurrent { get; set; }
        public IReadOnlyList<CHI_TIET_TRUYEN>? lstChapter { get; set; }
    }
}
