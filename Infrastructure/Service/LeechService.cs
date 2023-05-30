using HtmlAgilityPack;
using SayHenTai_WebApp.Infrastructure.Caching;
using SayHenTai_WebApp.Infrastructure.Data;
using SayHenTai_WebApp.Infrastructure.Entities;
using SayHenTai_WebApp.Models;
using System.Diagnostics;

namespace SayHenTai_WebApp.Infrastructure.Service
{
    public class LeechService
    {
        private readonly MemoryCacheService _memoryCacheService;
        private readonly LeechTruyenData _leechTruyenData;
        public LeechService(
           MemoryCacheService memoryCacheService,
           LeechTruyenData leechTruyenData)
        {
            _memoryCacheService = memoryCacheService;
            _leechTruyenData = leechTruyenData;
        }

        public virtual async Task<bool> LeechTruyenByUrl(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            var TenTruyenNode = doc.DocumentNode.SelectSingleNode("//div[@class='post-title']//h1");
            var imageCoverNode = doc.DocumentNode.SelectSingleNode("//div[@class='summary_image']//img");
            string tenTruyen = string.Empty;
            string imgCoverUrl = string.Empty;
            if (TenTruyenNode != null)
            {
                tenTruyen = TenTruyenNode.InnerHtml;
            }
            if (imageCoverNode != null)
            {
                imgCoverUrl = imageCoverNode.GetAttributeValue("src", "");
            }

            if (!string.IsNullOrEmpty(tenTruyen) && !string.IsNullOrEmpty(imgCoverUrl))
            {
                var Id = await _leechTruyenData.InsertThongTinTruyenAsync(tenTruyen, imgCoverUrl);

                //Get Toàn bộ list chapter để đưa vào bảng CHUONG_MUC
                HtmlNodeCollection ulList = doc.DocumentNode.SelectNodes("//ul[@class='list-item box-list-chapter limit-height']");
                List<HtmlNode> liList = new List<HtmlNode>();
                List<CHUONG_MUC> CptList = new List<CHUONG_MUC>();
                foreach (HtmlNode ul in ulList)
                {
                    HtmlNodeCollection liNodes = ul.SelectNodes(".//li");
                    if (liNodes != null)
                    {
                        liList.AddRange(liNodes);
                    }
                }
                foreach (HtmlNode li in liList)
                {
                    var model = new CHUONG_MUC();
                    var aNode = li.SelectSingleNode("./a");
                    var viewNumber = li.SelectSingleNode("./span[@class='number-view']");
                    var releasedate = li.SelectSingleNode("./span[@class='chapter-release-date']//i");
                    var ChapterName = aNode.InnerText;
                    var urlLinkChapter = aNode.GetAttributeValue("href", "");
                    var numView = viewNumber.InnerText.Replace(" ","").Replace("views", "");
                    model.TEN = ChapterName;
                    model.URL = urlLinkChapter;
                    model.SO_LAN_DOC = int.Parse(numView);
                    model.THOI_GIAN_CAP_NHAT = releasedate.InnerText;
                    model.ID_TRUYEN = Id;
                    CptList.Add(model);
                }

                var IsTaoChuongMuc = _leechTruyenData.InsertListChuongMuc(CptList);
                await Task.Delay(1000);
                //Lấy dữ liệu từ bảng chương mục
                var cacheKeyChuongMuc = CreateCacheKey.BuildDanhSachChuongMucByIdTruyenAsync(Id);
                var DsChuongMuc = await _memoryCacheService.GetOrCreate(cacheKeyChuongMuc, async () =>
                {
                    return await _leechTruyenData.GetDanhSachChuongMucTruyenByIdTruyen(Id);
                }, CoreConstants.DefaultCacheTimeDay);
                if(DsChuongMuc != null)
                {
                    List<CHI_TIET_TRUYEN> Chapter = new List<CHI_TIET_TRUYEN>();
                    foreach (var itemCM in DsChuongMuc)
                    {
                        HtmlWeb webChapter = new HtmlWeb();
                        HtmlDocument docChapter = webChapter.Load(itemCM.URL);
                        HtmlNodeCollection imgNodeList = docChapter.DocumentNode.SelectNodes("//div[@id='chapter_content']//img");
                        if (imgNodeList != null)
                        {
                            foreach (HtmlNode imgClass in imgNodeList)
                            {
                                var CptModel = new CHI_TIET_TRUYEN();
                                var imgUrl = imgClass.GetAttributeValue("src", "");
                                var imgWidth = imgClass.GetAttributeValue("width", "");
                                var imgHeght = imgClass.GetAttributeValue("height", "");
                                var imgAlt = imgClass.GetAttributeValue("alt", "");
                                var imgId = imgClass.GetAttributeValue("id", "");

                                CptModel.ID_CHUONG_MUC = itemCM.ID;
                                CptModel.ID_TRUYEN = itemCM.ID_TRUYEN;
                                CptModel.IMAGE_URL = imgUrl;
                                CptModel.WIDTH = imgWidth;
                                CptModel.HEIGHT = imgHeght;
                                CptModel.ALT = imgAlt;
                                CptModel.ID_SOURCE = imgId;
                                Chapter.Add(CptModel);
                            }
                        }
                    }
                    var insertTruyen = _leechTruyenData.InsertListChiTietTruyen(Chapter);
                }    
                return true;
            }    
            return false;
        }
    }
}
